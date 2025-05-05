using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services.Interfaces;
public interface ICategoryService
{
  Task<List<CategoryViewModel>> GetCategoriesForMenuAsync(int pageIndex, int pageSize);
  Task<CategoryViewModel> GetCategoryByIdAsync(Guid categoryId);
}
