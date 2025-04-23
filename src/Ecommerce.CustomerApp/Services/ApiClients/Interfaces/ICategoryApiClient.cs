
using Ecommerce.Shared.Common;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
public interface ICategoryApiClient
{
  Task<List<CategoryViewModel>> GetAllCategoriesAsync(int pageIndex, int pageSize);
  Task<CategoryViewModel> GetCategoryByIdAsync(Guid id);
}
