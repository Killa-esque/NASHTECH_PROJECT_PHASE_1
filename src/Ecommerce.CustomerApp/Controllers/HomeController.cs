using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
  public async Task<IActionResult> Index()
  {
    var accessToken = await HttpContext.GetTokenAsync("access_token");
    var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
    var idToken = await HttpContext.GetTokenAsync("id_token");
    var expiresAt = await HttpContext.GetTokenAsync("expires_at");
    return View();
  }
}
