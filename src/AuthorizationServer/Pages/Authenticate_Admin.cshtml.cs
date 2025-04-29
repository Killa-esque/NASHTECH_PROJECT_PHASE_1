// Pages/Authenticate_Admin.cshtml.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Ecommerce.Infrastructure.Entities;

namespace AuthorizationServer.Pages;

public class Authenticate_AdminModel : PageModel
{
  private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly UserManager<ApplicationUser> _userManager;

  public Authenticate_AdminModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
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
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };
    var roles = await _userManager.GetRolesAsync(user);
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var props = new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) };
    await HttpContext.SignInAsync("AdminScheme", new ClaimsPrincipal(new ClaimsIdentity(claims, "password")), props);

    return !string.IsNullOrEmpty(ReturnUrl) ? Redirect(ReturnUrl) : RedirectToPage("/Index");

    IActionResult InvalidLogin()
    {
      ModelState.AddModelError(string.Empty, "Invalid login attempt.");
      return Page();
    }
  }
}
// using System.ComponentModel.DataAnnotations;
// using Ecommerce.Infrastructure.Entities;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;

// namespace AuthorizationServer.Pages;

// public class Authenticate_AdminModel : PageModel
// {
//   private readonly SignInManager<ApplicationUser> _signInManager;
//   private readonly UserManager<ApplicationUser> _userManager;

//   public Authenticate_AdminModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
//   {
//     _signInManager = signInManager;
//     _userManager = userManager;
//   }

//   [BindProperty]
//   public string? ReturnUrl { get; set; }

//   [BindProperty]
//   public InputModel Input { get; set; } = new InputModel();

//   public class InputModel
//   {
//     [Required]
//     public string Email { get; set; }

//     [Required]
//     [DataType(DataType.Password)]
//     public string Password { get; set; }
//   }

//   public IActionResult OnGet(string returnUrl)
//   {
//     ReturnUrl = returnUrl;
//     return Page();
//   }

//   public async Task<IActionResult> OnPostAsync()
//   {
//     Console.WriteLine("[AUTH] 🟡 Bắt đầu xử lý login...");

//     if (!ModelState.IsValid)
//     {
//       Console.WriteLine("[AUTH] 🔴 ModelState không hợp lệ.");
//       return Page();
//     }

//     var user = await _userManager.FindByEmailAsync(Input.Email);
//     if (user == null)
//     {
//       Console.WriteLine("[AUTH] ❌ Không tìm thấy người dùng.");
//       return Page();
//     }

//     Console.WriteLine("[AUTH] ✅ Tìm thấy user, đang kiểm tra password...");

//     var result = await _signInManager.PasswordSignInAsync(user, Input.Password, isPersistent: true, lockoutOnFailure: false);

//     if (!result.Succeeded)
//     {
//       Console.WriteLine("[AUTH] ❌ Sai mật khẩu.");
//       return Page();
//     }

//     // Xác định scheme dựa trên returnUrl
//     var scheme = ReturnUrl?.Contains("client_id=customer_client") == true ? "CustomerScheme" : "AdminScheme";
//     await _signInManager.SignInAsync(user, isPersistent: true, authenticationMethod: scheme);

//     Console.WriteLine("[AUTH] ✅ Đăng nhập thành công.");

//     if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
//     {
//       Console.WriteLine($"[AUTH] 🔁 Redirect về ReturnUrl: {ReturnUrl}");
//       return LocalRedirect(ReturnUrl);
//     }

//     Console.WriteLine("[AUTH] ⏩ Không có ReturnUrl, chuyển về trang chính.");
//     return RedirectToPage("/Index");
//   }
// }
