using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services;

public class OrderService : IOrderService
{
  private readonly IOrderApiClient _orderApiClient;

  public OrderService(IOrderApiClient orderApiClient)
  {
    _orderApiClient = orderApiClient;
  }

  public async Task<ApiResponse<Guid>> CreateOrderAsync(CreateOrderViewModel order)
  {
    return await _orderApiClient.CreateOrderAsync(order);
  }

  public async Task<ApiResponse<OrderViewModel>> GetOrderDetailsAsync(Guid orderId)
  {
    return await _orderApiClient.GetOrderDetailsAsync(orderId);
  }

  public async Task<ApiResponse<PagedResult<OrderViewModel>>> GetUserOrdersAsync(int pageIndex, int pageSize)
  {
    return await _orderApiClient.GetUserOrdersAsync(pageIndex, pageSize);
  }

  public async Task<ApiResponse<bool>> CancelOrderAsync(Guid orderId)
  {
    return await _orderApiClient.CancelOrderAsync(orderId);
  }
  public async Task<ApiResponse<string>> GetOrderCodeAsync(Guid orderId)
  {
    return await _orderApiClient.GetOrderCodeAsync(orderId);
  }
}
