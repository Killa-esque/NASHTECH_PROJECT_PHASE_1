using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.ViewModels;
using System.Net.Http;
using System.Text.Json;

namespace Ecommerce.CustomerApp.Services.ApiClients;
public class CategoryApiClient : ICategoryApiClient
{
  private readonly HttpClient _httpClient;

  public CategoryApiClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<List<CategoryViewModel>> GetAllCategoriesAsync(int pageIndex, int pageSize)
  {
    var response = await _httpClient.GetAsync($"/categories?pageIndex={pageIndex}&pageSize={pageSize}");
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    var pagedResult = JsonSerializer.Deserialize<PagedResult<CategoryViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    return pagedResult?.Items.ToList() ?? new List<CategoryViewModel>();
  }

  public async Task<CategoryViewModel> GetCategoryByIdAsync(Guid id)
  {
    var response = await _httpClient.GetAsync($"/categories/{id}");
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    var category = JsonSerializer.Deserialize<CategoryViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    return category!;
  }
}
