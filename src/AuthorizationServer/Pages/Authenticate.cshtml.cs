using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthorizationServer.Pages;
public class AuthenticateModel : PageModel
{
  private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly UserManager<ApplicationUser> _userManager;

  public AuthenticateModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
  {
    _signInManager = signInManager;
    _userManager = userManager;
  }
  [BindProperty]
  public string? ReturnUrl { get; set; }

  [BindProperty]
  public InputModel Input { get; set; } = new InputModel();

  public class InputModel
  {
    [Required]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
  }
  public IActionResult OnGet(string returnUrl)
  {
    ReturnUrl = returnUrl;
    return Page();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    Console.WriteLine("[AUTH] 🟡 Bắt đầu xử lý login...");

    if (ModelState.IsValid == false)
    {
      Console.WriteLine("[AUTH] 🔴 ModelState không hợp lệ.");
      return Page();
    }

    var user = await _userManager.FindByEmailAsync(Input.Email);
    if (user == null)
    {
      Console.WriteLine("[AUTH] ❌ Không tìm thấy người dùng.");
      return Page();
    }

    Console.WriteLine("[AUTH] ✅ Tìm thấy user, đang kiểm tra password...");

    var result = await _signInManager.PasswordSignInAsync(user, Input.Password, isPersistent: true, lockoutOnFailure: false);

    if (!result.Succeeded)
    {
      Console.WriteLine("[AUTH] ❌ Sai mật khẩu.");
      return Page();
    }

    Console.WriteLine("[AUTH] ✅ Đăng nhập thành công.");

    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
    {
      Console.WriteLine($"[AUTH] 🔁 Redirect về ReturnUrl: {ReturnUrl}");
      return LocalRedirect(ReturnUrl);
    }

    Console.WriteLine("[AUTH] ⏩ Không có ReturnUrl, chuyển về trang chính.");
    return RedirectToPage("/Index");
  }
}



// public string Email { get; set; } = Consts.Email;
// public string Password { get; set; } = Consts.Password;

// [BindProperty]
// public string? ReturnUrl { get; set; }
// public string AuthStatus { get; set; } = "";

// public IActionResult OnGet(string returnUrl)
// {
//   ReturnUrl = returnUrl;
//   return Page();
// }

// public async Task<IActionResult> OnPostAsync(string email, string password)
// {
//   if (email != Consts.Email || password != Consts.Password)
//   {
//     AuthStatus = "Email or password is invalid";
//     return Page();
//   }

//   var claims = new List<Claim>
//           {
//               new(ClaimTypes.Email, email),
//           };

//   var principal = new ClaimsPrincipal(
//       new List<ClaimsIdentity>
//       {
//                   new(claims, IdentityConstants.ApplicationScheme)
//       });

//   await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

//   if (!string.IsNullOrEmpty(ReturnUrl))
//   {
//     return Redirect(ReturnUrl);
//   }

//   AuthStatus = "Successfully authenticated";
//   return Page();
// }
