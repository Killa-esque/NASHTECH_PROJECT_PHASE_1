using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

[Route("account")]
public class AccountController : Controller
{
  private readonly UserManager<IdentityUser> _userManager;

  public AccountController(UserManager<IdentityUser> userManager)
  {
    _userManager = userManager;
  }

  [HttpGet("login")]
  public IActionResult Login(string returnUrl = null)
  {
    ViewBag.ReturnUrl = returnUrl;
    return View();
  }

  [HttpPost("login")]
  public async Task<IActionResult> LoginPost(string email, string password, string returnUrl)
  {
    var user = await _userManager.FindByEmailAsync(email);

    Console.WriteLine($"User: {user}");
    Console.WriteLine($"Email: {email}");
    Console.WriteLine($"Password: {password}");
    Console.WriteLine($"ReturnUrl: {returnUrl}");

    if (user != null)
    {
      var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
      Console.WriteLine($"CheckPasswordAsync result: {isPasswordValid}");

      if (isPasswordValid)
      {
        var claims = new List<Claim>
        {
            new Claim(OpenIddictConstants.Claims.Subject, user.Id.ToString()),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        // Verify the result of SignInAsync
        var isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
        Console.WriteLine($"SignInAsync completed. IsAuthenticated: {isAuthenticated}");
        Console.WriteLine("Authenticated User.Claims:");
        foreach (var claim in HttpContext.User.Claims)
        {
          Console.WriteLine($"{claim.Type}: {claim.Value}");
        }

        Console.WriteLine("==== ĐÃ ĐĂNG NHẬP ====");
        Console.WriteLine("IsAuthenticated: " + User.Identity?.IsAuthenticated);
        Console.WriteLine("User.Claims: ");
        foreach (var c in User.Claims)
          Console.WriteLine($"{c.Type}: {c.Value}");

        return LocalRedirect(returnUrl ?? "/");
      }
    }

    return View("Login"); // Ensure a default return value
  }
}
