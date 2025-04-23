using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces.Repositories;

public interface ICartRepository
{
  Task<IEnumerable<CartItem>> GetCartItemsByUserAsync(Guid userId, int pageIndex, int pageSize);
  Task<int> AddOrUpdateCartItemAsync(Guid userId, Guid productId, int quantity);
  Task<int> RemoveCartItemAsync(Guid userId, Guid productId);
  Task<int> GetTotalCountByUserAsync(Guid userId);
}

