using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginModel : PageModel
{
  public IActionResult OnPost()
  {
    var redirectUrl = Url.Page("/Account/Callback");
    var props = new AuthenticationProperties { RedirectUri = redirectUrl };
    return Challenge(props, "oidc");
  }
}
