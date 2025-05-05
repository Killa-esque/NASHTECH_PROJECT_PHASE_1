using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.CustomerApp.Controllers;

public class CategoryController : Controller
{
  private readonly ICategoryService _categoryService;

  public CategoryController(ICategoryService categoryService)
  {
    _categoryService = categoryService;
  }

  public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
  {
    try
    {
      var categories = await _categoryService.GetCategoriesForMenuAsync(pageIndex, pageSize);
      return View(categories);
    }
    catch (Exception ex)
    {
      ViewBag.Error = $"Lỗi khi tải danh sách danh mục: {ex.Message}";
      return View(new List<CategoryViewModel>());
    }
  }
}
