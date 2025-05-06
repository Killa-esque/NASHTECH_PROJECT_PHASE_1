using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;

namespace Ecommerce.Application.Interfaces.Services;
public interface IOrderService
{
  Task<Result<PagedResult<OrderDto>>> GetAllOrdersAsync(int pageIndex, int pageSize);
  Task<Result<PagedResult<OrderDto>>> GetOrdersByCustomerIdAsync(string customerId, int pageIndex, int pageSize);
  Task<Result<Guid>> CreateOrderAsync(string userId, CreateOrderDto orderDto);
  Task<Result> CancelOrderAsync(Guid orderId);
  Task<Result> UpdateOrderAsync(Guid orderId, UpdateOrderDto updateOrderDto);
  Task<Result<string>> GetOrderStatusAsync(Guid orderId);
  Task<Result<OrderDto>> GetOrderDetailsAsync(Guid orderId);
  Task<Result<PagedResult<OrderDto>>> GetUserOrdersAsync(string userId, int pageIndex, int pageSize);
  Task<Result<string>> GetOrderCodeAsync(Guid orderId);
  Task<Result<bool>> DeleteOrdersByUserIdAsync(string userId);
}
