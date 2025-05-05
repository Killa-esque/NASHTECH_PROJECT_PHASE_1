using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;

public interface IOrderService
{
  Task<ApiResponse<Guid>> CreateOrderAsync(CreateOrderViewModel order);
  Task<ApiResponse<OrderViewModel>> GetOrderDetailsAsync(Guid orderId);
  Task<ApiResponse<PagedResult<OrderViewModel>>> GetUserOrdersAsync(int pageIndex, int pageSize);
  Task<ApiResponse<bool>> CancelOrderAsync(Guid orderId);
  Task<ApiResponse<string>> GetOrderCodeAsync(Guid orderId);
}
