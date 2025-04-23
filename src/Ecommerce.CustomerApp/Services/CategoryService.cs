using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services;
public class CategoryService : ICategoryService
{
  private readonly ICategoryApiClient _categoryApiClient;

  public CategoryService(ICategoryApiClient categoryApiClient)
  {
    _categoryApiClient = categoryApiClient;
  }

  public async Task<List<CategoryViewModel>> GetCategoriesForMenuAsync(int pageIndex = 1, int pageSize = 10)
  {
    var categories = await _categoryApiClient.GetAllCategoriesAsync(pageIndex, pageSize);
    // UI logic nếu cần: filter, sắp xếp...
    return categories.OrderBy(c => c.Name).ToList();
  }
}
