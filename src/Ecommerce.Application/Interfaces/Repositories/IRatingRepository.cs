using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces.Repositories;

public interface IRatingRepository
{
  Task<IEnumerable<Rating>> GetRatingsByProductIdAsync(Guid productId, int pageIndex = 1, int pageSize = 10);
  Task AddAsync(Rating rating);
  Task SaveChangesAsync();
  Task<string> GetUserNameByIdAsync(string userId);
  Task<int> CountRatingsByProductIdAsync(Guid productId);
}
