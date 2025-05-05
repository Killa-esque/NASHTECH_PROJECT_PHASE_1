using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;
using Microsoft.AspNetCore.Authentication;

public class OrderApiClient : IOrderApiClient
{
  private readonly HttpClient _httpClient;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public OrderApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
  {
    _httpClient = httpClient;
    _httpContextAccessor = httpContextAccessor;
    _httpClient.BaseAddress = new Uri("https://localhost:5001/");
  }

  private async Task AddAuthorizationHeaderAsync()
  {
    var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
    Console.WriteLine($"Access Token: {accessToken}");
    if (string.IsNullOrEmpty(accessToken))
    {
      throw new InvalidOperationException("Access token is missing.");
    }
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var userId = _httpContextAccessor.HttpContext.User.FindFirst("sub")?.Value ?? "a2ad953e-1482-4cf2-9b94-41c88aeb90e3";
    Console.WriteLine($"X-User-Id: {userId}");
    _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
  }

  public async Task<ApiResponse<Guid>> CreateOrderAsync(CreateOrderViewModel order)
  {
    Console.WriteLine($"Sending POST /api/orders with data: {JsonSerializer.Serialize(order)}");
    await AddAuthorizationHeaderAsync();
    var content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
    var response = await _httpClient.PostAsync("api/orders", content);
    var responseContent = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"CreateOrder Response Status: {response.StatusCode}");
    Console.WriteLine($"CreateOrder Response Content: { responseContent}");

    if (!response.IsSuccessStatusCode)
    {
      return ApiResponse<Guid>.Fail($"API call failed with status {response.StatusCode}: {responseContent}");
    }

    try
    {
      var result = JsonSerializer.Deserialize<ApiResponse<Guid>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      return result ?? ApiResponse<Guid>.Fail("Failed to create order.");
    }
    catch (JsonException ex)
    {
      return ApiResponse<Guid>.Fail($"Failed to parse response: {ex.Message}. Content: {responseContent}");
    }
  }

  public async Task<ApiResponse<string>> GetOrderCodeAsync(Guid orderId)
  {
    Console.WriteLine($"Sending GET /api/orders/code/{orderId}");
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.GetAsync($"api/orders/code/{orderId}");
    var responseContent = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"GetOrderCode Response Status: {response.StatusCode}");
    Console.WriteLine($"GetOrderCode Response Content: {responseContent}");

    if (!response.IsSuccessStatusCode)
    {
      return ApiResponse<string>.Fail($"API call failed with status {response.StatusCode}: {responseContent}");
    }

    try
    {
      var result = JsonSerializer.Deserialize<ApiResponse<string>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      return result ?? ApiResponse<string>.Fail("Failed to retrieve order code.");
    }
    catch (JsonException ex)
    {
      return ApiResponse<string>.Fail($"Failed to parse response: {ex.Message}. Content: {responseContent}");
    }
  }

  public async Task<ApiResponse<OrderViewModel>> GetOrderDetailsAsync(Guid orderId)
  {
    Console.WriteLine($"Sending GET /api/orders/{orderId}");
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.GetAsync($"api/orders/{orderId}");
    var responseContent = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"GetOrderDetails Response Status: {response.StatusCode}");
    Console.WriteLine($"GetOrderDetails Response Content: {responseContent}");

    if (!response.IsSuccessStatusCode)
    {
      return ApiResponse<OrderViewModel>.Fail($"API call failed with status {response.StatusCode}: {responseContent}");
    }

    try
    {
      var result = JsonSerializer.Deserialize<ApiResponse<OrderViewModel>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      return result ?? ApiResponse<OrderViewModel>.Fail("Failed to retrieve order details.");
    }
    catch (JsonException ex)
    {
      return ApiResponse<OrderViewModel>.Fail($"Failed to parse response: {ex.Message}. Content: {responseContent}");
    }
  }

  public async Task<ApiResponse<PagedResult<OrderViewModel>>> GetUserOrdersAsync(int pageIndex, int pageSize)
  {
    Console.WriteLine($"Sending GET /api/orders/my?pageIndex={pageIndex}&pageSize={pageSize}");
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.GetAsync($"api/orders/my?pageIndex={pageIndex}&pageSize={pageSize}");
    var responseContent = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"GetUserOrders Response Status: {response.StatusCode}");
    Console.WriteLine($"GetUserOrders Response Content: {responseContent}");

    if (!response.IsSuccessStatusCode)
    {
      return ApiResponse<PagedResult<OrderViewModel>>.Fail($"API call failed with status {response.StatusCode}: {responseContent}");
    }

    try
    {
      var result = JsonSerializer.Deserialize<ApiResponse<PagedResult<OrderViewModel>>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      return result ?? ApiResponse<PagedResult<OrderViewModel>>.Fail("Failed to retrieve user orders.");
    }
    catch (JsonException ex)
    {
      return ApiResponse<PagedResult<OrderViewModel>>.Fail($"Failed to parse response: {ex.Message}. Content: {responseContent}");
    }
  }

  public async Task<ApiResponse<bool>> CancelOrderAsync(Guid orderId)
  {
    Console.WriteLine($"Sending POST /api/orders/{orderId}/cancel");
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.PostAsync($"api/orders/{orderId}/cancel", null);
    var responseContent = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"CancelOrder Response Status: {response.StatusCode}");
    Console.WriteLine($"CancelOrder Response Content: {responseContent}");

    if (!response.IsSuccessStatusCode)
    {
      return ApiResponse<bool>.Fail($"API call failed with status {response.StatusCode}: {responseContent}");
    }

    try
    {
      var result = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      return result ?? ApiResponse<bool>.Fail("Failed to cancel order.");
    }
    catch (JsonException ex)
    {
      return ApiResponse<bool>.Fail($"Failed to parse response: {ex.Message}. Content: {responseContent}");
    }
  }
}
