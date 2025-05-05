using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.ApiClients;

public class CategoryApiClient : ICategoryApiClient
{
  private readonly HttpClient _httpClient;
  private readonly string _baseUrl;

  public CategoryApiClient(HttpClient httpClient, IConfiguration configuration)
  {
    _httpClient = httpClient;
    _baseUrl = configuration["ApiSettings:BaseUrl"];
  }

  public async Task<List<CategoryDto>> GetAllCategoriesAsync(int pageIndex, int pageSize)
  {
    var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<CategoryViewModel>>>(
        $"{_baseUrl}/api/categories?pageIndex={pageIndex}&pageSize={pageSize}");

    if (response?.Status != true || response.Data == null)
    {
      throw new Exception(response?.Message ?? "Lỗi khi lấy danh sách danh mục.");
    }

    return response.Data.Items.Select(c => new CategoryDto
    {
      Id = c.Id,
      Name = c.Name,
      Description = c.Description
    }).ToList();
  }

  public async Task<CategoryDto> GetCategoryByIdAsync(Guid categoryId)
  {
    var response = await _httpClient.GetFromJsonAsync<ApiResponse<CategoryViewModel>>(
        $"{_baseUrl}/api/categories/{categoryId}");

    if (response?.Status != true || response.Data == null)
    {
      throw new Exception(response?.Message ?? "Lỗi khi lấy chi tiết danh mục.");
    }

    return new CategoryDto
    {
      Id = response.Data.Id,
      Name = response.Data.Name,
      Description = response.Data.Description
    };
  }
}
