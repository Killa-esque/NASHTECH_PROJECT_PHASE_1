using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces.Repositories;

public interface ICartRepository
{
  Task<IEnumerable<CartItem>> GetCartItemsByUserAsync(string userId, int pageIndex, int pageSize);
  Task<int> AddOrUpdateCartItemAsync(string userId, Guid productId, int quantity, bool isUpdate = false);
  Task<int> RemoveCartItemAsync(string userId, Guid productId);
  Task<int> GetTotalCountByUserAsync(string userId);
  Task<int> ClearCartAsync(string userId);
}

