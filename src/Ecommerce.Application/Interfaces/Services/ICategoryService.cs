using Ecommerce.Shared.Common;
using Ecommerce.Application.DTOs;

namespace Ecommerce.Application.Interfaces.Services;

public interface ICategoryService
{
  Task<Result<PagedResult<CategoryDto>>> GetAllCategoriesAsync(int pageIndex, int pageSize);
  Task<Result<CategoryDto>> GetCategoryByIdAsync(Guid id);
  Task<Result> AddCategoryAsync(CategoryDto categoryDto);
  Task<Result> UpdateCategoryAsync(CategoryDto categoryDto);
  Task<Result> DeleteCategoryAsync(Guid id);
}
