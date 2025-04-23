using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services.Interfaces;

public interface IProductService
{
  Task<List<ProductViewModel>> GetProductsForCategoryPageAsync(Guid categoryId, int pageIndex, int pageSize);
  Task<ProductViewModel> GetProductDetailsAsync(Guid id);
}
