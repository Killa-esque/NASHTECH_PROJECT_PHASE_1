using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;

namespace Ecommerce.CustomerApp.Services.Interfaces;
public interface IProductApiClient
{
  Task<PagedResult<ProductDto>> GetAllProductsAsync(int pageIndex, int pageSize);
  Task<PagedResult<ProductDto>> GetFeaturedProductsAsync(int pageIndex, int pageSize);
  Task<PagedResult<ProductDto>> GetProductsForCategoryPageAsync(Guid categoryId, int pageIndex, int pageSize);
  Task<ProductDto> GetProductDetailsAsync(Guid productId);
}
