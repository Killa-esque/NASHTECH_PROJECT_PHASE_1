

using Ecommerce.CustomerApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.CustomerApp.Controllers;

public class ProductController : Controller
{
  public IActionResult Index()
  {
    return View();
  }

  public IActionResult Detail()
  {
    return View();
  }

  // public IActionResult Index(string? search)
  // {
  //   var products = _productService.GetAll();

  //   if (!string.IsNullOrWhiteSpace(search))
  //   {
  //     products = products
  //         .Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
  //   }

  //   return View(products);
  // }


}
