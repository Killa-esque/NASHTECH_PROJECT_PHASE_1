using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace CustomerApp.Pages.Account;

public class LoginModel : PageModel
{
  private readonly UserManager<IdentityUser> _userManager;
  private readonly SignInManager<IdentityUser> _signInManager;

  public LoginModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
  {
    _userManager = userManager;
    _signInManager = signInManager;
  }

  [BindProperty]
  public InputModel Input { get; set; } = new();

  [BindProperty(SupportsGet = true)]
  public string? ReturnUrl { get; set; }

  public class InputModel
  {
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
  }

  public void OnGet()
  {
    // Nếu người dùng đã đăng nhập, chuyển hướng về trang chủ
    if (User.Identity != null && User.Identity.IsAuthenticated)
    {
      Response.Redirect("/");
      return;
    }

    // Kiểm tra ReturnUrl
    if (!string.IsNullOrWhiteSpace(ReturnUrl) && !Url.IsLocalUrl(ReturnUrl))
    {
      ModelState.AddModelError(string.Empty, "Đường dẫn không hợp lệ.");
      ReturnUrl = "/"; // Đặt giá trị mặc định nếu ReturnUrl không hợp lệ
    }

    // Nếu không có ReturnUrl, đặt giá trị mặc định là trang chủ
    if (string.IsNullOrWhiteSpace(ReturnUrl))
    {
      ReturnUrl = "/";
    }
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
      return Page();

    var user = await _userManager.FindByEmailAsync(Input.Email);
    if (user == null || !await _userManager.CheckPasswordAsync(user, Input.Password))
    {
      ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
      return Page();
    }

    // Tạo cookie auth
    var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? ""),
            new(Claims.Subject, user.Id),
        };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
        new AuthenticationProperties
        {
          IsPersistent = true,
          ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        });

    // Nếu không có returnUrl thì quay về homepage
    if (string.IsNullOrWhiteSpace(ReturnUrl) || !Url.IsLocalUrl(ReturnUrl))
      ReturnUrl = "/";

    return LocalRedirect(ReturnUrl);
  }
}
