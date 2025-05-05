using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using System.Net;

namespace Ecommerce.CustomerApp.ApiClients;

public class ProductApiClient : IProductApiClient
{
  private readonly HttpClient _httpClient;
  private readonly string _baseUrl;
  private readonly ILogger<ProductApiClient> _logger;

  public ProductApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<ProductApiClient> logger)
  {
    _httpClient = httpClient;
    _baseUrl = configuration["ApiSettings:BaseUrl"];
    _logger = logger;
  }

  public async Task<PagedResult<ProductDto>> GetAllProductsAsync(int pageIndex, int pageSize)
  {
    try
    {
      var response = await _httpClient.GetAsync($"{_baseUrl}/api/products?pageIndex={pageIndex}&pageSize={pageSize}");
      response.EnsureSuccessStatusCode();
      var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<ProductDto>>>();

      if (result?.Status != true || result.Data == null)
      {
        _logger.LogError("Failed to get products: Status={Status}, Message={Message}", result?.Status, result?.Message);
        throw new Exception(result?.Message ?? "Lỗi khi lấy danh sách sản phẩm.");
      }

      return result.Data;
    }
    catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
      _logger.LogWarning("Product list not found: {Message}", ex.Message);
      return PagedResult<ProductDto>.Create(new List<ProductDto>(), 0, pageIndex, pageSize);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching product list");
      throw;
    }
  }

  public async Task<PagedResult<ProductDto>> GetFeaturedProductsAsync(int pageIndex, int pageSize)
  {
    try
    {
      var response = await _httpClient.GetAsync($"{_baseUrl}/api/products/featured?pageIndex={pageIndex}&pageSize={pageSize}");
      response.EnsureSuccessStatusCode();
      var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<ProductDto>>>();

      if (result?.Status != true || result.Data == null)
      {
        _logger.LogError("Failed to get featured products: Status={Status}, Message={Message}", result?.Status, result?.Message);
        throw new Exception(result?.Message ?? "Lỗi khi lấy sản phẩm nổi bật.");
      }

      return result.Data;
    }
    catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
      _logger.LogWarning("Featured products not found: {Message}", ex.Message);
      return PagedResult<ProductDto>.Create(new List<ProductDto>(), 0, pageIndex, pageSize);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching featured products");
      throw;
    }
  }

  public async Task<PagedResult<ProductDto>> GetProductsForCategoryPageAsync(Guid categoryId, int pageIndex, int pageSize)
  {
    try
    {
      var response = await _httpClient.GetAsync($"{_baseUrl}/api/products/category/{categoryId}?pageIndex={pageIndex}&pageSize={pageSize}");
      response.EnsureSuccessStatusCode();
      var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<ProductDto>>>();

      if (result?.Status != true || result.Data == null)
      {
        _logger.LogError("Failed to get products for category {CategoryId}: Status={Status}, Message={Message}", categoryId, result?.Status, result?.Message);
        throw new Exception(result?.Message ?? "Lỗi khi lấy sản phẩm theo danh mục.");
      }

      return result.Data;
    }
    catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
      _logger.LogWarning("Products for category {CategoryId} not found: {Message}", categoryId, ex.Message);
      return PagedResult<ProductDto>.Create(new List<ProductDto>(), 0, pageIndex, pageSize);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching products for category {CategoryId}", categoryId);
      throw;
    }
  }

  public async Task<ProductDto> GetProductDetailsAsync(Guid productId)
  {
    try
    {
      var response = await _httpClient.GetAsync($"{_baseUrl}/api/products/{productId}");
      if (response.StatusCode == HttpStatusCode.NotFound)
      {
        _logger.LogWarning("Product {ProductId} not found", productId);
        return null;
      }

      response.EnsureSuccessStatusCode();
      var result = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

      if (result?.Status != true || result.Data == null)
      {
        _logger.LogError("Failed to get product {ProductId}: Status={Status}, Message={Message}", productId, result?.Status, result?.Message);
        throw new Exception(result?.Message ?? "Lỗi khi lấy chi tiết sản phẩm.");
      }

      return result.Data;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, "HTTP error fetching product {ProductId}", productId);
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching product {ProductId}", productId);
      throw;
    }
  }
}
