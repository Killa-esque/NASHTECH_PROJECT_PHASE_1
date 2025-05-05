using Ecommerce.Shared.DTOs;

namespace Ecommerce.CustomerApp.Services.Interfaces;

public interface ICategoryApiClient
{
  Task<List<CategoryDto>> GetAllCategoriesAsync(int pageIndex, int pageSize);
  Task<CategoryDto> GetCategoryByIdAsync(Guid categoryId);
}
