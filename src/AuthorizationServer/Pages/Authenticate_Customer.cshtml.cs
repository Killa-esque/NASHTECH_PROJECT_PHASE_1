using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Ecommerce.Infrastructure.Entities;

namespace AuthorizationServer.Pages;

public class Authenticate_CustomerModel : PageModel
{
  private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly UserManager<ApplicationUser> _userManager;

  public Authenticate_CustomerModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
  {
    _signInManager = signInManager;
    _userManager = userManager;
  }

  [BindProperty] public string? ReturnUrl { get; set; }
  [BindProperty] public InputModel Input { get; set; } = new();

  public class InputModel
  {
    [Required] public string Email { get; set; }
    [Required, DataType(DataType.Password)] public string Password { get; set; }
  }

  public IActionResult OnGet(string returnUrl) => (ReturnUrl = returnUrl) != null ? Page() : Page();

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid) return Page();

    var user = await _userManager.FindByEmailAsync(Input.Email);
    if (user == null) return InvalidLogin();

    var result = await _signInManager.PasswordSignInAsync(user, Input.Password, true, false);
    if (!result.Succeeded) return InvalidLogin();

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, user.Email),
        new(ClaimTypes.Email, user.Email)
    };

    var roles = await _userManager.GetRolesAsync(user);
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var props = new AuthenticationProperties
    {
      IsPersistent = true,
      ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
    };

    // 👇 DÙNG SCHEME ĐÚNG
    var identity = new ClaimsIdentity(claims, "password");
    var principal = new ClaimsPrincipal(identity);
    await HttpContext.SignInAsync("CustomerScheme", principal, props);

    return !string.IsNullOrEmpty(ReturnUrl) ? Redirect(ReturnUrl) : RedirectToPage("/Index");

    IActionResult InvalidLogin()
    {
      ModelState.AddModelError(string.Empty, "Invalid login attempt.");
      return Page();
    }
  }
}
