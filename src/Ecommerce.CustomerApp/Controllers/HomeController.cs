using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
  private readonly ICategoryService _categoryService;

  public HomeController(ICategoryService categoryService)
  {
    _categoryService = categoryService;
  }

  public async Task<IActionResult> Index()
  {
    // var accessToken = await HttpContext.GetTokenAsync("access_token");
    // var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
    // var idToken = await HttpContext.GetTokenAsync("id_token");
    // var expiresAt = await HttpContext.GetTokenAsync("expires_at");
    // var categories = await _categoryService.GetCategoriesForMenuAsync(1, 10);

    // Console.WriteLine(categories.Count);

    return View();
  }
}
