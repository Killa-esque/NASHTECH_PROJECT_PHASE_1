using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.Requests;
using Ecommerce.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ecommerce.CustomerApp.Controllers;

[Authorize]
public class CartController : Controller
{
  private readonly ICartService _cartService;
  private readonly ILogger<CartController> _logger;

  public CartController(ICartService cartService, ILogger<CartController> logger)
  {
    _cartService = cartService;
    _logger = logger;
  }

  [HttpGet]
  public async Task<IActionResult> Index()
  {
    try
    {
      var cart = await _cartService.GetCartItemsAsync(1, 20);
      return View(cart.Items.ToList());
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error loading cart index");
      ViewBag.Error = $"Lỗi khi tải giỏ hàng: {ex.Message}";
      return View();
    }
  }

  [HttpGet]
  public async Task<IActionResult> GetCartSummary()
  {
    try
    {
      var cart = await _cartService.GetCartItemsAsync(1, 100);
      var totalItems = cart.Items.Sum(i => i.Quantity);
      return Json(new
      {
        success = true,
        totalItems,
        items = cart.Items.Select(i => new
        {
          i.ProductId,
          i.ProductName,
          i.Quantity,
          i.Price
        })
      });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching cart summary");
      return Json(new { success = false, message = ex.Message });
    }
  }

  [HttpPost]
  public async Task<IActionResult> AddToCart([FromBody] CartRequest request)
  {
    try
    {
      await _cartService.AddToCartAsync(request);
      var updatedCart = await _cartService.GetCartItemsAsync(1, 100);
      var totalItems = updatedCart.Items.Sum(i => i.Quantity);

      return Json(new { success = true, items = updatedCart.Items, totalItems });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error adding to cart for Product ID: {ProductId}", request.ProductId);
      return Json(new { success = false, message = ex.Message });
    }
  }

  [HttpPost]
  public async Task<IActionResult> Remove(Guid productId)
  {
    try
    {
      await _cartService.RemoveFromCartAsync(new RemoveFromCartRequest
      {
        ProductId = productId
      });
      var updatedCart = await _cartService.GetCartItemsAsync(1, 100);
      return Json(new { success = true, items = updatedCart.Items, totalItems = updatedCart.Items.Sum(i => i.Quantity) });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error removing product ID: {ProductId} from cart", productId);
      return Json(new { success = false, message = ex.Message });
    }
  }

  [HttpPost]
  public async Task<IActionResult> UpdateQuantity([FromBody] CartRequest request)
  {
    try
    {
      await _cartService.UpdateCartItemAsync(request);
      var updatedCart = await _cartService.GetCartItemsAsync(1, 100);
      return Json(new { success = true, items = updatedCart.Items, totalItems = updatedCart.Items.Sum(i => i.Quantity) });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating quantity for Product ID: {ProductId}", request.ProductId);
      return Json(new { success = false, message = ex.Message });
    }
  }
}
