using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Requests;

namespace Ecommerce.CustomerApp.Services.ApiClients.Interfaces;

public interface ICartApiClient
{
  Task<PagedResult<CartItemDto>> GetCartItemsAsync(int pageIndex, int pageSize);
  Task AddToCartAsync(CartRequest request);
  Task RemoveFromCartAsync(RemoveFromCartRequest request);
  Task UpdateCartItemAsync(CartRequest request);
}
