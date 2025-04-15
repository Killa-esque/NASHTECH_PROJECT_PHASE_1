using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CallbackModel : PageModel
{
  public async Task<IActionResult> OnGetAsync()
  {
    var result = await HttpContext.AuthenticateAsync("Cookies");

    if (!result.Succeeded || result.Principal == null)
    {
      return RedirectToPage("/Account/Login");
    }

    var accessToken = await HttpContext.GetTokenAsync("access_token");

    var name = result.Principal.FindFirst("name")?.Value;
    var email = result.Principal.FindFirst("email")?.Value;
    var sub = result.Principal.FindFirst("sub")?.Value;

    HttpContext.Session.SetString("Username", name ?? email ?? sub ?? "Guest");

    // ✅ Redirect sau khi login thành công
    return RedirectToPage("/Index");
  }
}
