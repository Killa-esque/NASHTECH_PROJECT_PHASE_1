using System.Collections.Immutable;
using System.Security.Claims;
using System.Web;
using AuthorizationServer.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthorizationServer.Controllers;

[ApiController]
public class AuthorizationServer : Controller
{
  private readonly IOpenIddictApplicationManager _applicationManager;
  private readonly IOpenIddictScopeManager _scopeManager;
  private readonly AuthorizationService _authService;


  public AuthorizationServer(IOpenIddictApplicationManager applicationManager,
    IOpenIddictScopeManager scopeManager,
    AuthorizationService authService)
  {
    _applicationManager = applicationManager;
    _scopeManager = scopeManager;
    _authService = authService;
  }

  [HttpGet("~/connect/authorize")]
  [HttpPost("~/connect/authorize")]
  public async Task<IActionResult> Authorize()
  {
    var request = HttpContext.GetOpenIddictServerRequest() ??
                  throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

    var parameters = _authService.ParseOAuthParameters(HttpContext, new List<string> { Parameters.Prompt });

    var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

    if (!_authService.IsAuthenticated(result, request))
    {
      return Challenge(properties: new AuthenticationProperties
      {
        RedirectUri = _authService.BuildRedirectUrl(HttpContext.Request, parameters)
      }, [IdentityConstants.ApplicationScheme]);
    }

    var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                      throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

    var consentType = await _applicationManager.GetConsentTypeAsync(application);

    if (consentType != ConsentTypes.Explicit)
    {
      return Forbid(
          authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
          properties: new AuthenticationProperties(new Dictionary<string, string?>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidClient,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                  "Only explicit consent clients are supported"
          }));
    }

    var consentClaim = result.Principal.GetClaim(Consts.ConsentNaming);
    Console.WriteLine($"[CONSENT] ✅ consentClaim: {consentClaim}");

    if (consentClaim != Consts.GrantAccessValue)
    {
      var returnUrl = HttpUtility.UrlEncode(_authService.BuildRedirectUrl(HttpContext.Request, parameters));
      var consentRedirectUrl = $"/Consent?returnUrl={returnUrl}";

      return Redirect(consentRedirectUrl);
    }

    var userId = result.Principal.FindFirst(ClaimTypes.Email)!.Value;

    var identity = new ClaimsIdentity(
        authenticationType: TokenValidationParameters.DefaultAuthenticationType,
        nameType: Claims.Name,
        roleType: Claims.Role);

    identity.SetClaim(Claims.Subject, userId)
        .SetClaim(Claims.Email, userId)
        .SetClaim(Claims.Name, userId)
        .SetClaims(Claims.Role, new List<string> { "user", "admin" }.ToImmutableArray());

    identity.SetScopes(request.GetScopes());
    identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

    identity.SetDestinations(c => AuthorizationService.GetDestinations(identity, c));

    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
  }

  [HttpPost("~/connect/token")]
  public async Task<IActionResult> Exchange()
  {
    var request = HttpContext.GetOpenIddictServerRequest() ??
                  throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

    if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
      throw new InvalidOperationException("The specified grant type is not supported.");

    var result =
        await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    var userId = result.Principal.GetClaim(Claims.Subject);

    if (string.IsNullOrEmpty(userId))
    {
      return Forbid(
          authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
          properties: new AuthenticationProperties(new Dictionary<string, string?>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                  "Cannot find user from the token."
          }));
    }

    var identity = new ClaimsIdentity(result.Principal.Claims,
        authenticationType: TokenValidationParameters.DefaultAuthenticationType,
        nameType: Claims.Name,
        roleType: Claims.Role);

    identity.SetClaim(Claims.Subject, userId)
        .SetClaim(Claims.Email, userId)
        .SetClaim(Claims.Name, userId)
        .SetClaims(Claims.Role, new List<string> { "user", "admin" }.ToImmutableArray());

    identity.SetDestinations(c => AuthorizationService.GetDestinations(identity, c));

    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
  }

  [HttpGet("~/connect/logout")]
  [HttpPost("~/connect/logout")]
  public async Task<IActionResult> LogoutPost()
  {
    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

    return SignOut(
        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
        properties: new AuthenticationProperties
        {
          RedirectUri = "/"
        });
  }
}
