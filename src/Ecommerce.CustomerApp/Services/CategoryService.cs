using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services;

public class CategoryService : ICategoryService
{
  private readonly ICategoryApiClient _categoryApiClient;
  private readonly ILogger<CategoryService> _logger;

  public CategoryService(ICategoryApiClient categoryApiClient, ILogger<CategoryService> logger)
  {
    _categoryApiClient = categoryApiClient;
    _logger = logger;
  }

  public async Task<List<CategoryViewModel>> GetCategoriesForMenuAsync(int pageIndex, int pageSize)
  {
    _logger.LogInformation("Lấy danh mục: pageIndex={PageIndex}, pageSize={PageSize}", pageIndex, pageSize);
    var categoryDtos = await _categoryApiClient.GetAllCategoriesAsync(pageIndex, pageSize);
    _logger.LogInformation("Nhận được {CategoryCount} danh mục.", categoryDtos.Count);
    return categoryDtos.Select(dto => new CategoryViewModel
    {
      Id = dto.Id,
      Name = dto.Name,
      Description = dto.Description
    }).ToList();
  }

  public async Task<CategoryViewModel> GetCategoryByIdAsync(Guid categoryId)
  {
    _logger.LogInformation("Lấy chi tiết danh mục {CategoryId}.", categoryId);
    var dto = await _categoryApiClient.GetCategoryByIdAsync(categoryId);
    _logger.LogInformation("Nhận được danh mục: {CategoryName}.", dto.Name);
    return new CategoryViewModel
    {
      Id = dto.Id,
      Name = dto.Name,
      Description = dto.Description
    };
  }
}
