using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.CustomerApp.Controllers;

public class AccountController : Controller
{
  [HttpGet("/login")]
  public IActionResult Login(string returnUrl = "/")
  {
    return Challenge(new AuthenticationProperties
    {
      RedirectUri = returnUrl
    }, OpenIdConnectDefaults.AuthenticationScheme);
  }

  [HttpGet("/logout")]
  public IActionResult Logout()
  {
    return SignOut(new AuthenticationProperties
    {
      RedirectUri = "/"
    }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
  }
}
