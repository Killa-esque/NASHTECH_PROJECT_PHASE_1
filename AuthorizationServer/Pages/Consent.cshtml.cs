using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenIddict.Abstractions;

namespace AuthorizationServer.Pages;

[Authorize]
public class ConsentModel : PageModel
{
  [BindProperty]
  public string? ReturnUrl { get; set; }

  public IActionResult OnGet(string returnUrl)
  {
    ReturnUrl = returnUrl;
    return Page();
  }

  public async Task<IActionResult> OnPostAsync(string grant)
  {
    User.SetClaim(Consts.ConsentNaming, grant);

    var principal = new ClaimsPrincipal(new ClaimsIdentity(User.Claims, IdentityConstants.ApplicationScheme));

    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

    return Redirect(ReturnUrl ?? "/");
  }

}
