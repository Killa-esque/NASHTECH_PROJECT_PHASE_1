using Ecommerce.Shared.Common;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services.Interfaces;

public interface IProductService
{
  Task<PagedResult<ProductViewModel>> GetAllProductsAsync(int pageIndex, int pageSize);
  Task<PagedResult<ProductViewModel>> GetFeaturedProductsAsync(int pageIndex, int pageSize);
  Task<PagedResult<ProductViewModel>> GetProductsForCategoryPageAsync(Guid categoryId, int pageIndex, int pageSize);
  Task<ProductViewModel> GetProductDetailsAsync(Guid productId);
}
