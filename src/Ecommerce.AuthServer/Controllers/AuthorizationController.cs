using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;

public class AuthorizationController : Controller
{
  private readonly SignInManager<IdentityUser> _signInManager;
  private readonly UserManager<IdentityUser> _userManager;

  public AuthorizationController(SignInManager<IdentityUser> signInManager,
      UserManager<IdentityUser> userManager)
  {
    _signInManager = signInManager;
    _userManager = userManager;
  }

  [HttpGet("~/connect/authorize")]
  [HttpPost("~/connect/authorize")]
  [IgnoreAntiforgeryToken]
  public async Task<IActionResult> Authorize()
  {
    var request = HttpContext.GetOpenIddictServerRequest() ??
        throw new InvalidOperationException("The request is not an OpenID Connect authorization request.");

    Console.WriteLine($"Request: {request}");
    Console.WriteLine($"Request.ClientId: {request.ClientId}");
    // check  this User.Identity is { IsAuthenticated: true }
    Console.WriteLine($"User.Identity.IsAuthenticated: {User}");
    foreach (var claim in User.Claims)
    {
      Console.WriteLine($"{claim.Type} = {claim.Value}");
    }

    // Nếu user đã đăng nhập, tạo identity và trả về authorization code
    if (User.Identity is { IsAuthenticated: true })
    {
      var user = await _userManager.GetUserAsync(User);
      Console.WriteLine($"User: {user}");
      Console.WriteLine($"User.Identity.IsAuthenticated: {User.Identity.IsAuthenticated}");
      if (user == null)
      {
        return Challenge(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
      }

      var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

      identity.AddClaim(OpenIddictConstants.Claims.Subject, await _userManager.GetUserIdAsync(user));
      var userName = await _userManager.GetUserNameAsync(user);
      if (!string.IsNullOrEmpty(userName))
      {
        identity.AddClaim(OpenIddictConstants.Claims.Name, userName);
      }
      var email = await _userManager.GetEmailAsync(user);
      if (!string.IsNullOrEmpty(email))
      {
        identity.AddClaim(OpenIddictConstants.Claims.Email, email);
      }

      var principal = new ClaimsPrincipal(identity);

      principal.SetScopes(request.GetScopes());
      if (!string.IsNullOrEmpty(request.ClientId))
      {
        principal.SetResources(await GetResourcesForApplicationAsync(request.ClientId));
      }

      return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    // Nếu user chưa đăng nhập, redirect đến trang login
    return Challenge(
        authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
        properties: new AuthenticationProperties
        {
          RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
        });
  }

  private async Task<IEnumerable<string>> GetResourcesForApplicationAsync(string clientId)
  {
    // Logic để lấy danh sách resource server mà client được phép truy cập
    // Ví dụ: query database
    return new[] { "resource_server_1", "resource_server_2" };
  }
}
