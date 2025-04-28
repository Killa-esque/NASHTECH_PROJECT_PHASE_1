using Ecommerce.Shared.Common;
using Ecommerce.Application.DTOs;

namespace Ecommerce.Application.Services.Interfaces;

public interface ICartService
{
  Task<Result<PagedResult<CartItemDto>>> GetCartItemsAsync(string userId, int pageIndex, int pageSize);
  Task<Result> AddToCartAsync(string userId, Guid productId, int quantity);
  Task<Result> RemoveFromCartAsync(string userId, Guid productId);
}

