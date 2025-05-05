using Ecommerce.Shared.Common;
using Ecommerce.Shared.Requests;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services.Interfaces;

public interface ICartService
{
  Task<PagedResult<CartItemViewModel>> GetCartItemsAsync(int pageIndex, int pageSize);
  Task AddToCartAsync(CartRequest request);
  Task UpdateCartItemAsync(CartRequest request);
  Task RemoveFromCartAsync(RemoveFromCartRequest request);
}
