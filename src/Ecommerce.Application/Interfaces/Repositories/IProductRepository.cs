using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces.Repositories;

public interface IProductRepository
{
  Task<IEnumerable<Product>> GetFeaturedAsync(int pageIndex, int pageSize);
  Task<IEnumerable<Product>> GetAllAsync(int pageIndex, int pageSize);
  Task<Product?> GetByIdAsync(Guid id);
  Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, int pageIndex, int pageSize);
  Task<IEnumerable<ProductImage>> GetProductImagesAsync(Guid productId);
  Task<int> AddAsync(Product product);
  Task<int> UpdateAsync(Product product);
  Task<int> DeleteAsync(Guid id);
  Task<bool> ExistsAsync(string name, Guid? id = null);
  Task<int> CountAsync();
  Task<int> CountByCategoryAsync(Guid categoryId);
  Task<int> CountFeaturedProductsAsync();
  Task<IEnumerable<Product>> GetByIdsAsync(List<Guid> ids);
}
