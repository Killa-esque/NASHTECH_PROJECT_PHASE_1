using System.Collections.Immutable;
using System.Security.Claims;
using System.Web;
using AuthorizationServer.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Ecommerce.Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Controllers;

[ApiController]
public class AuthorizationController : Controller
{
  private readonly IOpenIddictApplicationManager _applicationManager;
  private readonly IOpenIddictScopeManager _scopeManager;
  private readonly AuthorizationService _authService;
  private readonly UserManager<ApplicationUser> _userManager;

  public AuthorizationController(
      IOpenIddictApplicationManager applicationManager,
      IOpenIddictScopeManager scopeManager,
      AuthorizationService authService,
      UserManager<ApplicationUser> userManager)
  {
    _applicationManager = applicationManager;
    _scopeManager = scopeManager;
    _authService = authService;
    _userManager = userManager;
  }

  /// <summary>
  /// Handles the OpenID Connect authorization request and issues an authorization code.
  /// </summary>
  [HttpGet("~/connect/authorize")]
  [HttpPost("~/connect/authorize")]
  public async Task<IActionResult> Authorize()
  {
    var request = HttpContext.GetOpenIddictServerRequest() ??
                  throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

    Console.WriteLine("[AUTH] Request Scopes: " + string.Join(", ", request.GetScopes()));

    var parameters = _authService.ParseOAuthParameters(HttpContext, new List<string> { Parameters.Prompt });

    // test schema
    var clientId = request.ClientId;
    var scheme = clientId switch
    {
      "admin_client" => "AdminScheme",
      "customer_client" => "CustomerScheme",
      _ => IdentityConstants.ApplicationScheme
    };

    // Authenticate the user using ASP.NET Core Identity
    var result = await HttpContext.AuthenticateAsync(scheme);
    // var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
    Console.WriteLine($"[AUTH] Authorize: Authenticated result: {result?.Succeeded}, Principal: {result?.Principal?.Identity?.Name}");

    if (!_authService.IsAuthenticated(result, request))
    {
      Console.WriteLine("[AUTH] Authorize: User not authenticated, redirecting to login...");
      return Challenge(properties: new AuthenticationProperties
      {
        RedirectUri = _authService.BuildRedirectUrl(HttpContext.Request, parameters)
      }, [scheme]);
      // return Challenge(properties: new AuthenticationProperties
      // {
      //   RedirectUri = _authService.BuildRedirectUrl(HttpContext.Request, parameters)
      // }, [IdentityConstants.ApplicationScheme]);
    }

    // Get user ID from the authenticated principal (using email as identifier)
    var userId = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
    if (string.IsNullOrEmpty(userId))
    {
      Console.WriteLine("[AUTH] Authorize: User ID (email) not found in principal");
      return Forbid(
          authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
          properties: new AuthenticationProperties(new Dictionary<string, string?>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User ID not found in token."
          }));
    }

    // Find user by email
    var user = await _userManager.FindByEmailAsync(userId);
    if (user == null)
    {
      Console.WriteLine($"[AUTH] Authorize: User with email {userId} not found");
      return Forbid(
          authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
          properties: new AuthenticationProperties(new Dictionary<string, string?>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User not found."
          }));
    }

    // Get user roles
    var roles = await _userManager.GetRolesAsync(user);
    if (!roles.Any())
    {
      roles = new List<string> { "guest" };
    }

    // Check if the user has the required role for the client
    if (clientId == "admin_client" && !roles.Contains("admin"))
    {
      Console.WriteLine($"[AUTH] Authorize: User {userId} lacks 'admin' role for admin_client");
      return Forbid(
          authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
          properties: new AuthenticationProperties(new Dictionary<string, string?>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.AccessDenied,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User lacks required role."
          }));
    }
    else if (clientId == "customer_client" && !roles.Contains("customer"))
    {
      Console.WriteLine($"[AUTH] Authorize: User {userId} lacks 'customer' role for customer_client");
      return Forbid(
          authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
          properties: new AuthenticationProperties(new Dictionary<string, string?>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.AccessDenied,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User lacks required role."
          }));
    }

    // Validate client application
    var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                      throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

    var consentType = await _applicationManager.GetConsentTypeAsync(application);
    Console.WriteLine($"[AUTH] Consent type: {consentType}");

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

    // Check for user consent
    var consentClaim = result.Principal.GetClaim(Consts.ConsentNaming);
    Console.WriteLine($"[AUTH] Consent claim: {consentClaim}");

    if (consentClaim != Consts.GrantAccessValue)
    {
      var returnUrl = HttpUtility.UrlEncode(_authService.BuildRedirectUrl(HttpContext.Request, parameters));
      var consentRedirectUrl = $"/Consent?returnUrl={returnUrl}";
      Console.WriteLine($"[AUTH] Redirecting to consent page: {consentRedirectUrl}");
      return Redirect(consentRedirectUrl);
    }

    // Create a new ClaimsIdentity for the token
    var identity = new ClaimsIdentity(
        authenticationType: TokenValidationParameters.DefaultAuthenticationType,
        nameType: Claims.Name,
        roleType: Claims.Role);

    identity.SetClaim(Claims.Subject, user.Id) // Use user.Id for sub
        .SetClaim(Claims.Email, user.Email)    // Use user.Email for email
        .SetClaim(Claims.Name, user.UserName)  // Use user.UserName for name
        .SetClaims(Claims.Role, roles.ToImmutableArray());

    identity.SetScopes(request.GetScopes());
    identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

    identity.SetDestinations(c => AuthorizationService.GetDestinations(identity, c));

    Console.WriteLine($"[AUTH] Authorize: Signing in with user {user.Id}, roles: {string.Join(", ", roles)}");
    var principal = new ClaimsPrincipal(identity);
    var signInResult = SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    Console.WriteLine($"[AUTH] Authorize: SignIn result created, redirecting to client...");
    return signInResult;
  }

  /// <summary>
  /// Exchanges an authorization code or refresh token for an access token and id token.
  /// </summary>
  [HttpPost("~/connect/token")]
  public async Task<IActionResult> Exchange()
  {
    var request = HttpContext.GetOpenIddictServerRequest() ??
                  throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

    if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
      throw new InvalidOperationException("The specified grant type is not supported.");

    var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    if (!result.Succeeded)
    {
      Console.WriteLine("[AUTH] Exchange: Authentication failed");
      return Forbid(
          authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
          properties: new AuthenticationProperties(new Dictionary<string, string?>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Authentication failed."
          }));
    }

    var userId = result.Principal.GetClaim(Claims.Subject);
    if (string.IsNullOrEmpty(userId))
    {
      Console.WriteLine("[AUTH] Exchange: User ID (subject) not found in token");
      return Forbid(
          authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
          properties: new AuthenticationProperties(new Dictionary<string, string?>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Cannot find user from the token."
          }));
    }

    // Find user by ID (not email, since Claims.Subject is user.Id)
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      Console.WriteLine($"[AUTH] Exchange: User with ID {userId} not found");
      return Forbid(
          authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
          properties: new AuthenticationProperties(new Dictionary<string, string?>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User not found."
          }));
    }

    // Get user roles
    var roles = await _userManager.GetRolesAsync(user);
    if (!roles.Any())
    {
      roles = new List<string> { "guest" };
    }

    Console.WriteLine($"[AUTH] Exchange: User {userId} roles: {string.Join(", ", roles)}");

    // Create a new ClaimsIdentity for the token
    var identity = new ClaimsIdentity(
        authenticationType: TokenValidationParameters.DefaultAuthenticationType,
        nameType: Claims.Name,
        roleType: Claims.Role);

    identity.SetClaim(Claims.Subject, user.Id)
        .SetClaim(Claims.Email, user.Email)
        .SetClaim(Claims.Name, user.UserName)
        .SetClaims(Claims.Role, roles.ToImmutableArray());

    identity.SetDestinations(c => AuthorizationService.GetDestinations(identity, c));

    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
  }

  /// <summary>
  /// Returns user information based on the access token provided in the request.
  /// </summary>
  [HttpGet("~/connect/userinfo")]
  public async Task<IActionResult> UserInfo()
  {
    var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    if (!result.Succeeded)
    {
      Console.WriteLine("[AUTH] UserInfo: Authentication failed");
      return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    var userId = result.Principal.GetClaim(Claims.Subject);
    if (string.IsNullOrEmpty(userId))
    {
      Console.WriteLine("[AUTH] UserInfo: User ID (subject) not found in token");
      return Forbid(
          authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
          properties: new AuthenticationProperties(new Dictionary<string, string?>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User ID not found in token."
          }));
    }

    // Find user by ID
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      Console.WriteLine($"[AUTH] UserInfo: User with ID {userId} not found");
      return NotFound(new { error = "User not found." });
    }

    var claims = new Dictionary<string, object>
    {
      ["sub"] = user.Id,
      ["email"] = user.Email,
      ["name"] = user.UserName,
      ["roles"] = await _userManager.GetRolesAsync(user)
    };

    return Ok(claims);
  }

  /// <summary>
  /// Logs out the user and redirects to the specified URI.
  /// </summary>
  [HttpGet("~/connect/logout")]
  [HttpPost("~/connect/logout")]
  public async Task<IActionResult> LogoutPost([FromQuery] string post_logout_redirect_uri)
  {
    Console.WriteLine("[AUTH] Logout: Signing out user...");

    if (HttpContext.User.Identity.IsAuthenticated)
    {
      await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
    }

    // Nếu không có post_logout_redirect_uri thì fallback default
    if (string.IsNullOrEmpty(post_logout_redirect_uri))
    {
      post_logout_redirect_uri = "https://localhost:3000/signin"; // Default fallback
    }

    // Validate post_logout_redirect_uri against registered PostLogoutRedirectUris
    var request = HttpContext.GetOpenIddictServerRequest();
    if (request?.ClientId != null)
    {
      var application = await _applicationManager.FindByClientIdAsync(request.ClientId);
      if (application != null)
      {
        var postLogoutRedirectUris = await _applicationManager.GetPostLogoutRedirectUrisAsync(application);
        if (!postLogoutRedirectUris.Contains(post_logout_redirect_uri))
        {
          Console.WriteLine($"[AUTH] Logout: Invalid post_logout_redirect_uri: {post_logout_redirect_uri}");
          return BadRequest(new { error = "Invalid post_logout_redirect_uri." });
        }
      }
    }



    Console.WriteLine($"[AUTH] Redirecting to: {post_logout_redirect_uri}");
    return Redirect(post_logout_redirect_uri);
  }

}
