using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.CustomerApp.Controllers;

public class OrderController : Controller
{
  private readonly IOrderService _orderService;
  private readonly ICartService _cartService;

  public OrderController(IOrderService orderService, ICartService cartService)
  {
    _cartService = cartService;
    _orderService = orderService;
  }

  public async Task<IActionResult> Index()
  {
    try
    {
      var cartItems = await _cartService.GetCartItemsAsync(1, 100);
      var orderItems = cartItems.Items.Select(item => new OrderItemViewModel
      {
        Id = item.ProductId,
        ProductName = item.ProductName,
        Quantity = item.Quantity,
        Price = item.Price
      }).ToList();

      var orderViewModel = new OrderViewModel
      {
        Items = orderItems,
        TotalAmount = orderItems.Sum(i => i.Price * i.Quantity),
        ShippingAddress = string.Empty,
        PaymentMethod = string.Empty,
        Note = string.Empty,
        Status = string.Empty,
        CreatedDate = DateTime.Now,
        DeliveryDate = null,
        OrderCode = string.Empty
      };

      Console.WriteLine($"OrderViewModel: {orderViewModel.TotalAmount}");
      Console.WriteLine($"OrderViewModel Items: {string.Join(", ", orderViewModel.Items.Select(i => $"{i.ProductName} - {i.Quantity}"))}");

      return View(orderViewModel);
    }
    catch (Exception ex)
    {
      TempData["Error"] = $"Lỗi khi tải giỏ hàng: {ex.Message}";
      return RedirectToAction("Index", "Cart");
    }
  }

  public async Task<IActionResult> Detail(Guid id)
  {
    try
    {
      Console.WriteLine($"Order ID: {id}");

      if (id == Guid.Empty)
      {
        TempData["Error"] = "ID đơn hàng không hợp lệ.";
        return RedirectToAction("History");
      }

      var response = await _orderService.GetOrderDetailsAsync(id);
      if (!response.Status)
      {
        TempData["Error"] = response.Message;
        return RedirectToAction("History");
      }

      return View(response.Data);
    }
    catch (Exception ex)
    {
      TempData["Error"] = ex.Message;
      return RedirectToAction("History");
    }
  }

  public async Task<IActionResult> Success(Guid orderId)
  {
    try
    {
      var response = await _orderService.GetOrderCodeAsync(orderId);
      Console.WriteLine($"OrderCode: {response.Data}");

      var model = new SuccessViewModel
      {
        OrderCode = response.Status ? response.Data : "N/A",
        OrderId = orderId,
        Message = response.Status ? "Đặt hàng thành công." : "Có lỗi xảy ra khi lấy mã đơn hàng."
      };

      return View(model);
    }
    catch (Exception ex)
    {
      TempData["Error"] = ex.Message;
      return RedirectToAction("Index");
    }
  }

  public async Task<IActionResult> History(int pageIndex = 1, int pageSize = 10)
  {
    try
    {
      var response = await _orderService.GetUserOrdersAsync(pageIndex, pageSize);
      if (!response.Status)
      {
        TempData["Error"] = response.Message;
        return View(new PagedResult<OrderViewModel>());
      }

      return View(response.Data);
    }
    catch (Exception ex)
    {
      TempData["Error"] = ex.Message;
      return View(new PagedResult<OrderViewModel>());
    }
  }

  [HttpPost]
  public async Task<IActionResult> Submit([FromBody] CreateOrderViewModel order)
  {
    if (!ModelState.IsValid || order.Items == null || !order.Items.Any())
    {
      var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
      Console.WriteLine($"ModelState Errors: {string.Join("; ", errors)}");
      return Json(ApiResponse<string>.Fail($"Dữ liệu không hợp lệ. Errors: {string.Join("; ", errors)}"));
    }

    try
    {      
      var response = await _orderService.CreateOrderAsync(order);

     if (!response.Status)
      {
        Console.WriteLine($"CreateOrderAsync failed: {response.Message}");
        return Json(ApiResponse<string>.Fail(response.Message));
      }

      if (response.Data == Guid.Empty)
      {
        Console.WriteLine("CreateOrderAsync returned empty Order ID");
        return Json(ApiResponse<string>.Fail("Không thể tạo đơn hàng: ID đơn hàng không hợp lệ."));
      }

      // HttpContext.Session.Remove("Cart");
      return Json(ApiResponse<Guid>.Success(response.Data, "Đặt hàng thành công."));
    }
    catch (Exception ex)
    {
      Console.WriteLine($"SubmitOrder exception: {ex.Message}");
      return Json(ApiResponse<string>.Fail(ex.Message));
    }
  }

  [HttpPost]
  public async Task<IActionResult> Cancel([FromBody] CancelOrderViewModel order)
  {
    Console.WriteLine($"CancelOrder: {order.OrderId}");
    try
    {
      if (order.OrderId == Guid.Empty)
      {
        Console.WriteLine("CancelOrder: Invalid order ID received.");
        return Json(ApiResponse<string>.Fail("ID đơn hàng không hợp lệ."));
      }

      var response = await _orderService.CancelOrderAsync(order.OrderId);
      if (!response.Status)
      {
        Console.WriteLine($"CancelOrder failed: {response.Message}");
        return Json(ApiResponse<string>.Fail(response.Message));
      }

      return Json(ApiResponse<string>.Success("Hủy đơn hàng thành công."));
    }
    catch (Exception ex)
    {
      Console.WriteLine($"CancelOrder exception: {ex.Message}");
      return Json(ApiResponse<string>.Fail(ex.Message));
    }
  }
}
