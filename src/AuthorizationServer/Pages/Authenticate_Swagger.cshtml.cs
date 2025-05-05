using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OAuth.OpenIddict.AuthorizationServer.Pages;
public class Authenticate_SwaggerModel : PageModel
{
  [BindProperty] public string? ReturnUrl { get; set; }
  [BindProperty] public InputModel Input { get; set; } = new();

  public class InputModel
  {
    [Required] public string Email { get; set; }
    [Required, DataType(DataType.Password)] public string Password { get; set; }
  }

  public IActionResult OnGet(string returnUrl)
  {
    Console.WriteLine($"[DEBUG] ReturnUrl: {returnUrl}");
    ReturnUrl = string.IsNullOrEmpty(returnUrl)
        ? "https://localhost:5001/swagger/oauth2-redirect.html"
        : returnUrl;

    return Page();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid) return Page();

    // Sử dụng email và password cố định
    if (Input.Email != "pzinh@gmail.com" || Input.Password != "Password123@")
    {
      ModelState.AddModelError(string.Empty, "Invalid login attempt.");
      return Page();
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, Input.Email),
        new(ClaimTypes.Email, Input.Email)
    };

    var props = new AuthenticationProperties
    {
      IsPersistent = true,
      ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
    };

    // Sử dụng SwaggerScheme
    var identity = new ClaimsIdentity(claims, "password");
    var principal = new ClaimsPrincipal(identity);
    await HttpContext.SignInAsync("SwaggerScheme", principal, props);

    Console.WriteLine("[SEED] ✅ Swagger user authenticated successfully.", ReturnUrl);

    // Sử dụng Redirect thay vì RedirectToPage
    return !string.IsNullOrEmpty(ReturnUrl) ? Redirect(ReturnUrl) : Redirect("https://localhost:5001/swagger/oauth2-redirect.html");
  }
}
