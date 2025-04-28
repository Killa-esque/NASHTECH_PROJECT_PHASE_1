using Ecommerce.Shared.Common;
using Ecommerce.Application.DTOs;
using Ecommerce.Shared.Common;

namespace Ecommerce.Application.Interfaces.Services;

public interface IProductService
{
  Task<Result<PagedResult<ProductDto>>> GetFeaturedProductsAsync(int pageIndex, int pageSize);
  Task<Result<PagedResult<ProductDto>>> GetProductsByCategoryAsync(Guid categoryId, int pageIndex, int pageSize);
  Task<Result<PagedResult<ProductDto>>> GetAllProductsAsync(int pageIndex, int pageSize);
  Task<Result<ProductDto>> GetProductByIdAsync(Guid id);
  Task<Result> AddProductAsync(ProductDto productDto);
  Task<Result> UpdateProductAsync(ProductDto productDto);
  Task<Result> DeleteProductAsync(Guid id);
  Task<Result> SetProductFeaturedAsync(Guid id, bool isFeatured);
}
