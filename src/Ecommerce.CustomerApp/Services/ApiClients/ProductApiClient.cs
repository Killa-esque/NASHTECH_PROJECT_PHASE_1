using System.Text.Json;
using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services.ApiClients;

public class ProductApiClient : IProductApiClient
{
  private readonly HttpClient _httpClient;

  public ProductApiClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<List<ProductViewModel>> GetProductsByCategoryAsync(Guid categoryId, int pageIndex, int pageSize)
  {
    var response = await _httpClient.GetAsync($"/products?categoryId={categoryId}&pageIndex={pageIndex}&pageSize={pageSize}");
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    var pagedResult = JsonSerializer.Deserialize<PagedResult<ProductViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    return pagedResult?.Items.ToList() ?? new List<ProductViewModel>();
  }

  public async Task<ProductViewModel?> GetProductByIdAsync(Guid id)
  {
    var response = await _httpClient.GetAsync($"/products/{id}");
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    var product = JsonSerializer.Deserialize<ProductViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    return product;
  }
}
