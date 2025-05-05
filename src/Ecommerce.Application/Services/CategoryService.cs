using AutoMapper;
using Ecommerce.Shared.DTOs;
using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Common;

namespace Ecommerce.Application.Services;

public class CategoryService : ICategoryService
{
  private readonly ICategoryRepository _categoryRepository;
  private readonly IMapper _mapper;

  public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
  {
    _categoryRepository = categoryRepository;
    _mapper = mapper;
  }
  public async Task<Result<PagedResult<CategoryDto>>> GetAllCategoriesAsync(int pageIndex, int pageSize)
  {
    var categories = await _categoryRepository.GetAllAsync(pageIndex, pageSize);

    var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);

    var totalCount = await _categoryRepository.CountAsync();

    var pagedResult = PagedResult<CategoryDto>.Create(categoriesDto, totalCount, pageIndex, pageSize);

    return Result.Success(pagedResult, "Categories retrieved successfully.");
  }

  public async Task<Result<CategoryDto>> GetCategoryByIdAsync(Guid id)
  {
    var categoryResult = await _categoryRepository.GetByIdAsync(id);
    if (categoryResult == null)
    {
      return Result.Failure<CategoryDto>("Category not found.");
    }

    var categoryDto = _mapper.Map<CategoryDto>(categoryResult);
    return Result.Success(categoryDto, "Category retrieved successfully.");
  }

  public async Task<Result> AddCategoryAsync(CategoryDto categoryDto)
  {
    if (categoryDto == null)
      return Result.Failure("Category cannot be null.");

    var category = _mapper.Map<Category>(categoryDto);

    var exists = await _categoryRepository.ExistsAsync(category.Name);
    if (exists)
      return Result.Failure("Category already exists.");

    var affectedRows = await _categoryRepository.AddAsync(category);

    if (affectedRows != 1)
      return Result.Failure("Failed to add category. Unexpected row count.");

    return Result.Success("Category added successfully.");
  }

  public async Task<Result> UpdateCategoryAsync(CategoryDto categoryDto)
  {
    if (categoryDto == null)
      return Result.Failure("Category data cannot be null.");

    var existingCategory = await _categoryRepository.GetByIdAsync(categoryDto.Id);
    if (existingCategory == null)
      return Result.Failure("Category does not exist.");

    _mapper.Map(categoryDto, existingCategory);

    var affectedRows = await _categoryRepository.UpdateAsync(existingCategory);

    if (affectedRows != 1)
      return Result.Failure("Failed to update category.");

    return Result.Success("Category updated successfully.");
  }

  public async Task<Result> DeleteCategoryAsync(Guid id)
  {
    var existingCategory = await _categoryRepository.GetByIdAsync(id);
    if (existingCategory == null)
      return Result.Failure("Category does not exist.");

    var affectedRows = await _categoryRepository.DeleteAsync(id);

    if (affectedRows != 1)
      return Result.Failure("Failed to delete category.");

    return Result.Success("Category deleted successfully.");
  }

}
