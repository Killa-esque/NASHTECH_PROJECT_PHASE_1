using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Requests;
using Ecommerce.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.CustomerApp.Controllers;

public class ProductController : Controller
{
  private readonly IProductService _productService;
  private readonly ICategoryService _categoryService;
  private readonly IRatingService _ratingService;
  private readonly ICartService _cartService;
  private readonly ILogger<ProductController> _logger;

  public ProductController(
      IProductService productService,
      ICategoryService categoryService,
      IRatingService ratingService,
      ICartService cartService,
      ILogger<ProductController> logger)
  {
    _productService = productService;
    _categoryService = categoryService;
    _ratingService = ratingService;
    _cartService = cartService;
    _logger = logger;
  }

  public async Task<IActionResult> Index(string? search, Guid? categoryId, string? sort, int pageIndex = 1, int pageSize = 12)
  {
    try
    {
      PagedResult<ProductViewModel> pagedResult;

      if (categoryId.HasValue)
      {
        pagedResult = await _productService.GetProductsForCategoryPageAsync(categoryId.Value, pageIndex, pageSize);
      }
      else
      {
        pagedResult = await _productService.GetAllProductsAsync(pageIndex, pageSize);
      }

      if (!string.IsNullOrWhiteSpace(search))
      {
        var filteredItems = pagedResult.Items
            .Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var totalCount = filteredItems.Count;
        var items = filteredItems
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        pagedResult = PagedResult<ProductViewModel>.Create(items, totalCount, pageIndex, pageSize);
      }

      switch (sort)
      {
        case "price-asc":
          pagedResult.Items = pagedResult.Items.OrderBy(p => p.Price).ToList();
          break;
        case "price-desc":
          pagedResult.Items = pagedResult.Items.OrderByDescending(p => p.Price).ToList();
          break;
        case "popular":
          pagedResult.Items = pagedResult.Items.OrderByDescending(p => p.Id).ToList();
          break;
        default:
          pagedResult.Items = pagedResult.Items.OrderByDescending(p => p.Id).ToList();
          break;
      }

      ViewBag.Categories = await _categoryService.GetCategoriesForMenuAsync(1, 20);
      ViewBag.CategoryId = categoryId;
      ViewBag.Sort = sort;
      ViewBag.Search = search;
      ViewBag.CurrentPage = pageIndex;
      ViewBag.TotalPages = (int)Math.Ceiling((double)pagedResult.TotalCount / pageSize);
      ViewBag.TotalItems = pagedResult.TotalCount;

      return View(pagedResult.Items);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error loading product index");
      ViewBag.Error = $"Lỗi khi tải sản phẩm: {ex.Message}";
      return View(new List<ProductViewModel>());
    }
  }

  public async Task<IActionResult> Detail(Guid id)
  {
    try
    {
      var product = await _productService.GetProductDetailsAsync(id);
      if (product == null)
      {
        _logger.LogWarning("Product not found for ID: {Id}", id);
        return View("NotFound", new { Message = "Sản phẩm không tồn tại." });
      }

      var ratings = await _ratingService.GetRatingsByProductAsync(id);
      var relatedProducts = await _productService.GetProductsForCategoryPageAsync(product.CategoryId, 1, 4);

      ViewBag.Ratings = ratings;
      ViewBag.RelatedProducts = relatedProducts;

      return View(product);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error loading product details for ID: {Id}", id);
      return View("NotFound", new { Message = $"Lỗi khi tải chi tiết sản phẩm: {ex.Message}" });
    }
  }

  [HttpPost]
  public async Task<IActionResult> Rate(CreateRatingDto request)
  {
    if (!ModelState.IsValid)
    {
      var product = await _productService.GetProductDetailsAsync(request.ProductId);
      if (product == null)
      {
        return View("NotFound", new { Message = "Sản phẩm không tồn tại." });
      }
      var ratings = await _ratingService.GetRatingsByProductAsync(request.ProductId);
      ViewBag.Ratings = ratings;
      return View("Detail", product);
    }

    try
    {
      await _ratingService.CreateRatingAsync(request);
      return RedirectToAction("Detail", new { id = request.ProductId });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error submitting rating for Product ID: {ProductId}", request.ProductId);
      var product = await _productService.GetProductDetailsAsync(request.ProductId);
      if (product == null)
      {
        return View("NotFound", new { Message = "Sản phẩm không tồn tại." });
      }
      var ratings = await _ratingService.GetRatingsByProductAsync(request.ProductId);
      ViewBag.Ratings = ratings;
      ViewBag.Error = $"Lỗi khi gửi đánh giá: {ex.Message}";
      return View("Detail", product);
    }
  }

  [HttpPost]
  public async Task<IActionResult> AddToCart([FromBody] CartRequest request)
  {
    try
    {
      await _cartService.AddToCartAsync(request);
      var updatedCart = await _cartService.GetCartItemsAsync(1, 100);
      return Json(new { success = true, items = updatedCart.Items, totalItems = updatedCart.Items.Sum(i => i.Quantity) });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error adding to cart for Product ID: {ProductId}", request.ProductId);
      return Json(new { success = false, message = ex.Message });
    }
  }
}
