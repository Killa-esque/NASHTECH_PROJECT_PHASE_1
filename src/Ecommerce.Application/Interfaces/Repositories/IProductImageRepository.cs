using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces.Repositories;

public interface IProductImageRepository
{
  Task AddRangeAsync(IEnumerable<ProductImage> images);
  Task<ProductImage?> GetByProductIdAndUrlAsync(Guid productId, string imageUrl);
  Task DeleteAsync(Guid id);
}

