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
    var scheme = ReturnUrl?.Contains("client_id=customer_client") == true
        ? "CustomerScheme"
        : ReturnUrl?.Contains("client_id=swagger_client") == true
        ? "SwaggerScheme"
        : "AdminScheme";

    var identity = new ClaimsIdentity(User.Claims, scheme);
    identity.AddClaim(new Claim(Consts.ConsentNaming, grant)); // 👈 Add consent claim

    var principal = new ClaimsPrincipal(identity);
    await HttpContext.SignInAsync(scheme, principal); // 👈 Use correct scheme

    return Redirect(ReturnUrl ?? "/");
  }


  // public async Task<IActionResult> OnPostAsync(string grant)
  // {
  //   User.SetClaim(Consts.ConsentNaming, grant);

  //   var scheme = ReturnUrl?.Contains("client_id=customer_client") == true ? "CustomerScheme" : "AdminScheme";
  //   var principal = new ClaimsPrincipal(new ClaimsIdentity(User.Claims, scheme));

  //   await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

  //   return Redirect(ReturnUrl ?? "/");
  // }

}
