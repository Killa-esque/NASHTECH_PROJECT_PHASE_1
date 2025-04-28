using Ecommerce.Shared.ViewModels;

public interface ICategoryService
{
  Task<List<CategoryViewModel>> GetCategoriesForMenuAsync(int pageIndex, int pageSize);
}
