using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
  Task<IEnumerable<Category>> GetAllAsync(int pageIndex, int pageSize);
  Task<Category> GetByIdAsync(Guid id);
  Task<bool> ExistsAsync(string name);
  Task<int> AddAsync(Category category);
  Task<int> UpdateAsync(Category category);
  Task<int> DeleteAsync(Guid id);
  Task<int> CountAsync();
}
