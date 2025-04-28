using AutoMapper;
using Ecommerce.Shared.Common;
using Ecommerce.Application.DTOs;
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
    return BadRequest(ApiResponse<string>.Fail(result.Error));
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
    var result = await _orderService.CancelOrderAsync(orderId);

    if (result.IsSuccess)
      return Ok(ApiResponse<string>.Success(null, result.Message));
    return BadRequest(ApiResponse<string>.Fail(result.Error));
  }

  // Helper lấy userId từ token
  private string GetUserId()
  {
    return "b831506c-d805-4b6b-8682-74892b7f86e7";
    // return Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
  }
}
