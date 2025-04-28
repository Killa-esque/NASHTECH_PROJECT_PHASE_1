using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
public interface IProductApiClient
{
  Task<List<ProductViewModel>> GetProductsByCategoryAsync(Guid categoryId, int pageIndex, int pageSize);
  Task<ProductViewModel?> GetProductByIdAsync(Guid id);
}
