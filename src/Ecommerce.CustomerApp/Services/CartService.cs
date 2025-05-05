using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.Requests;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services;

public class CartService : ICartService
{
  private readonly ICartApiClient _cartApiClient;

  public CartService(ICartApiClient cartApiClient)
  {
    _cartApiClient = cartApiClient;
  }

  public async Task<PagedResult<CartItemViewModel>> GetCartItemsAsync(int pageIndex, int pageSize)
  {
    var pagedDto = await _cartApiClient.GetCartItemsAsync(pageIndex, pageSize);

    var items = pagedDto.Items.Select(dto => new CartItemViewModel
    {
      Id = dto.Id,
      ProductId = dto.ProductId,
      ProductName = dto.ProductName,
      Quantity = dto.Quantity,
      Price = dto.Price,
      Total = dto.Price * dto.Quantity,
      ImageUrl = dto.ImageUrl
    }).ToList();

    return PagedResult<CartItemViewModel>.Create(items, pagedDto.TotalCount, pageIndex, pageSize);
  }

  public async Task AddToCartAsync(CartRequest request)
  {
    await _cartApiClient.AddToCartAsync(request);
  }

  public async Task RemoveFromCartAsync(RemoveFromCartRequest request)
  {
    await _cartApiClient.RemoveFromCartAsync(request);
  }

  public async Task UpdateCartItemAsync(CartRequest request)
  {
    await _cartApiClient.UpdateCartItemAsync(request);
  }
}
