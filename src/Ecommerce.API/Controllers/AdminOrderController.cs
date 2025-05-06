using AutoMapper;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
public class AdminOrderController : ControllerBase
{
  private readonly IOrderService _orderService;
  private readonly IMapper _mapper;

  public AdminOrderController(IOrderService orderService, IMapper mapper)
  {
    _orderService = orderService;
    _mapper = mapper;
  }

  // 1. Lấy danh sách đơn hàng

  [HttpGet]
  public async Task<IActionResult> GetAllOrdersByCustomerId([FromQuery] string customerId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    var result = await _orderService.GetOrdersByCustomerIdAsync(customerId, pageIndex, pageSize); // Bạn cần thêm service method này nếu chưa có

    if (result.IsSuccess)
      return Ok(ApiResponse<PagedResult<OrderDto>>.Success(result.Data, "Orders retrieved successfully."));
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

  // 3. Cập nhật trạng thái đơn hàng
  [HttpPost("{orderId}/status")]
  public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderDto dto)
  {
    var result = await _orderService.UpdateOrderAsync(orderId, dto);

    if (result.IsSuccess)
      return Ok(ApiResponse<string>.Success(result.Message));
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


}
