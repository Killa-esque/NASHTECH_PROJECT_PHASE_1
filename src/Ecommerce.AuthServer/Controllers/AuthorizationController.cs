using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;



namespace Ecommerce.API.Controllers;
public class AuthorizationController : Controller
{
  private readonly SignInManager<IdentityUser> _signInManager;
  private readonly UserManager<IdentityUser> _userManager;

  public AuthorizationController(
      SignInManager<IdentityUser> signInManager,
      UserManager<IdentityUser> userManager)
  {
    _signInManager = signInManager;
    _userManager = userManager;
  }

  [HttpGet("~/connect/authorize")]
  public async Task<IActionResult> Authorize(OpenIddictRequest request)
  {
    if (!User.Identity.IsAuthenticated)
    {
      return Challenge(
          authenticationSchemes: IdentityConstants.ApplicationScheme,
          properties: new AuthenticationProperties
          {
            RedirectUri = Request.Path + QueryString.Create(Request.Query)
          });
    }

    var user = await _userManager.GetUserAsync(User);
    var principal = await _signInManager.CreateUserPrincipalAsync(user);

    principal.SetScopes(request.GetScopes());
    principal.SetResources("ecommerce-api");

    return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
  }
}
