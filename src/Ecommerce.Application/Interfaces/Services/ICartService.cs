using Ecommerce.Application.Common;
using Ecommerce.Application.DTOs;
using Ecommerce.Shared.Common;

namespace Ecommerce.Application.Services.Interfaces;

public interface ICartService
{
  Task<Result<PagedResult<CartItemDto>>> GetCartItemsAsync(Guid userId, int pageIndex, int pageSize);
  Task<Result> AddToCartAsync(Guid userId, Guid productId, int quantity);
  Task<Result> RemoveFromCartAsync(Guid userId, Guid productId);
}

