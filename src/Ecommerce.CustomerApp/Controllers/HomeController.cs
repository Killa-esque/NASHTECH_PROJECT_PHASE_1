using Ecommerce.CustomerApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.CustomerApp.Controllers;

public class HomeController : Controller
{
  private readonly ICategoryService _categoryService;
  private readonly IProductService _productService;

  public HomeController(ICategoryService categoryService, IProductService productService)
  {
    _categoryService = categoryService;
    _productService = productService;
  }

  public async Task<IActionResult> Index(int page = 1)
  {
    var categories = await _categoryService.GetCategoriesForMenuAsync(1, 10);
    var featuredProductsPaged = await _productService.GetFeaturedProductsAsync(page, 8);

    ViewBag.Categories = categories;
    ViewBag.CurrentPage = featuredProductsPaged.PageIndex;
    ViewBag.TotalPages = (int)Math.Ceiling((double)featuredProductsPaged.TotalCount / featuredProductsPaged.PageSize);

    return View(featuredProductsPaged.Items);
  }

}
