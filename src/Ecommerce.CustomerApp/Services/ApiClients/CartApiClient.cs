using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Requests;
using Microsoft.AspNetCore.Authentication;

namespace Ecommerce.CustomerApp.Services.ApiClients;

public class CartApiClient : ICartApiClient
{
  private readonly HttpClient _httpClient;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CartApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
  {
    _httpClient = httpClient;
    _httpContextAccessor = httpContextAccessor;
  }

  private async Task AddAuthorizationHeaderAsync()
  {
    var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
    if (!string.IsNullOrEmpty(accessToken))
    {
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
  }

  public async Task<PagedResult<CartItemDto>> GetCartItemsAsync(int pageIndex, int pageSize)
  {
    await AddAuthorizationHeaderAsync();

    var response = await _httpClient.GetAsync($"/api/cart?pageIndex={pageIndex}&pageSize={pageSize}");
    response.EnsureSuccessStatusCode();

    var result = JsonSerializer.Deserialize<ApiResponse<PagedResult<CartItemDto>>>(
        await response.Content.ReadAsStringAsync(),
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    return result!.Data;
  }

  public async Task AddToCartAsync(CartRequest request)
  {
    await AddAuthorizationHeaderAsync();
    var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
    var response = await _httpClient.PostAsync("/api/cart/add", content);
    response.EnsureSuccessStatusCode();
  }

  public async Task RemoveFromCartAsync(RemoveFromCartRequest request)
  {
    await AddAuthorizationHeaderAsync();
    var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
    var response = await _httpClient.PostAsync("/api/cart/remove", content);
    response.EnsureSuccessStatusCode();
  }

  public async Task UpdateCartItemAsync(CartRequest request)
  {
    await AddAuthorizationHeaderAsync();
    var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
    var response = await _httpClient.PutAsync("/api/cart/update", content);
    response.EnsureSuccessStatusCode();
  }
}
