using System.Net;
using System.Net.Http.Headers;
using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Microsoft.AspNetCore.Authentication;

public class RatingApiClient : IRatingApiClient
{
  private readonly HttpClient _httpClient;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly string _baseUrl;
  private readonly ILogger<RatingApiClient> _logger;

  public RatingApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ILogger<RatingApiClient> logger)
  {
    _httpClient = httpClient;
    _httpContextAccessor = httpContextAccessor;
    _baseUrl = configuration["ApiSettings:BaseUrl"];
    _logger = logger;
  }

  private async Task AddAuthorizationHeaderAsync()
  {
    var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
    if (!string.IsNullOrEmpty(accessToken))
    {
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
  }

  public async Task<Guid> CreateRatingAsync(CreateRatingDto request)
  {
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/ratings", request);

    var result = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
    if (response.IsSuccessStatusCode && result?.Status == true)
    {
      return result.Data;
    }

    throw new Exception(result?.Message ?? "Không thể tạo đánh giá.");
  }
  public async Task<PagedResult<RatingDto>> GetRatingsByProductAsync(Guid productId)
  {
    try
    {
      var response = await _httpClient.GetAsync($"{_baseUrl}/api/ratings/product/{productId}");
      if (response.StatusCode == HttpStatusCode.NotFound)
      {
        _logger.LogWarning("No ratings found for product {ProductId}", productId);
        return PagedResult<RatingDto>.Create(new List<RatingDto>(), 0, 1, 10);
      }

      response.EnsureSuccessStatusCode();
      var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<RatingDto>>>();

      if (result?.Status != true || result.Data == null)
      {
        _logger.LogError("Failed to get ratings for product {ProductId}: Status={Status}, Message={Message}", productId, result?.Status, result?.Message);
        throw new Exception(result?.Message ?? "Lỗi khi lấy đánh giá sản phẩm.");
      }

      return result.Data;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, "HTTP error fetching ratings for product {ProductId}", productId);
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching ratings for product {ProductId}", productId);
      throw;
    }
  }
}
