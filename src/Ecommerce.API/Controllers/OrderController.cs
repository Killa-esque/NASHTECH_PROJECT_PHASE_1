using AutoMapper;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/orders")]
// [Authorize]
public class OrderController : ControllerBase
{
  private readonly IOrderService _orderService;
  private readonly IMapper _mapper;

  public OrderController(IOrderService orderService, IMapper mapper)
  {
    _orderService = orderService;
    _mapper = mapper;
  }

  // 1. Tạo đơn hàng
  [HttpPost]
  public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
  {
    var userId = GetUserId();
    var result = await _orderService.CreateOrderAsync(userId, dto);

    if (result.IsSuccess)
      return Ok(ApiResponse<Guid>.Success(result.Data, result.Message));
    return BadRequest(ApiResponse<Guid>.Fail(result.Error));
  }

  // 2. Xem chi tiết đơn hàng
  [HttpGet("{orderId}")]
  public async Task<IActionResult> GetOrderDetail(Guid orderId)
  {

    var result = await _orderService.GetOrderDetailsAsync(orderId);

    if (result.IsSuccess)
      return Ok(ApiResponse<OrderDto>.Success(result.Data, result.Message));
    return NotFound(ApiResponse<string>.Fail(result.Error));
  }

  // 3. Xem đơn hàng của chính mình
  [HttpGet("my")]
  public async Task<IActionResult> GetMyOrders([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    var userId = GetUserId();
    var result = await _orderService.GetUserOrdersAsync(userId, pageIndex, pageSize);

    if (result.IsSuccess)
      return Ok(ApiResponse<PagedResult<OrderDto>>.Success(result.Data, result.Message));
    return BadRequest(ApiResponse<string>.Fail(result.Error));
  }

  // 4. Hủy đơn hàng
  [HttpPost("{orderId}/cancel")]
  public async Task<IActionResult> CancelOrder(Guid orderId)
  {
    Console.WriteLine($"GetOrderDetail: {orderId}");

    var result = await _orderService.CancelOrderAsync(orderId);

    if (result.IsSuccess)
      return Ok(ApiResponse<string>.Success(null, result.Message));
    return BadRequest(ApiResponse<string>.Fail(result.Error));
  }

  // Get order code
  [HttpGet("code/{orderId}")]
  public async Task<IActionResult> GetOrderCode(Guid orderId)
  {
    var result = await _orderService.GetOrderCodeAsync(orderId);

    if (result.IsSuccess)
      return Ok(ApiResponse<string>.Success(result.Data, result.Message));
    return NotFound(ApiResponse<string>.Fail(result.Error));
  }

  // Helper lấy userId từ token
  private string GetUserId()
  {
    return "a8a9b907-81a6-41c9-90ff-eb2158c52044";
    // return Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
  }
}
