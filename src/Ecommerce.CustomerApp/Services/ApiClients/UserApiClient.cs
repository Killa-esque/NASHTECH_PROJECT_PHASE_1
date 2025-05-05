using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace Ecommerce.CustomerApp.Services.ApiClients;

public class UserApiClient : IUserApiClient
{
  private readonly HttpClient _httpClient;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public UserApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
  {
    _httpClient = httpClient;
    _httpContextAccessor = httpContextAccessor;
    _httpClient.BaseAddress = new Uri("https://localhost:5001/"); // URL của ResourceServer
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

  public async Task<ApiResponse<CustomerDto>> GetProfileAsync()
  {
    Console.WriteLine("Sending GET api/users/profile");
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.GetAsync("api/users/profile");
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"GetProfile Response Status: {response.StatusCode}");
    Console.WriteLine($"GetProfile Response Content: {content}");

    if (!response.IsSuccessStatusCode)
    {
      return ApiResponse<CustomerDto>.Fail($"API call failed with status {response.StatusCode}: {content}");
    }

    try
    {
      var result = JsonSerializer.Deserialize<ApiResponse<CustomerDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      return result ?? ApiResponse<CustomerDto>.Fail("Failed to retrieve profile.");
    }
    catch (JsonException ex)
    {
      Console.WriteLine($"GetProfile Deserialize Error: {ex.Message}");
      return ApiResponse<CustomerDto>.Fail($"Failed to parse response: {ex.Message}. Content: {content}");
    }
  }

  public async Task<ApiResponse<CustomerDto>> UpdateProfileAsync(UpdateCustomerDto updateDto)
  {
    Console.WriteLine($"Sending PUT api/users/profile with data: {JsonSerializer.Serialize(updateDto)}");
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.PutAsJsonAsync("api/users/profile", updateDto);
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"UpdateProfile Response Status: {response.StatusCode}");
    Console.WriteLine($"UpdateProfile Response Content: {content}");

    if (!response.IsSuccessStatusCode)
    {
      return ApiResponse<CustomerDto>.Fail($"API call failed with status {response.StatusCode}: {content}");
    }

    try
    {
      var result = JsonSerializer.Deserialize<ApiResponse<CustomerDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      return result ?? ApiResponse<CustomerDto>.Fail("Failed to update profile.");
    }
    catch (JsonException ex)
    {
      Console.WriteLine($"UpdateProfile Deserialize Error: {ex.Message}");
      return ApiResponse<CustomerDto>.Fail($"Failed to parse response: {ex.Message}. Content: {content}");
    }
  }

  public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
  {
    Console.WriteLine($"Sending POST api/users/change-password with data: {JsonSerializer.Serialize(changePasswordDto)}");
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.PostAsJsonAsync("api/users/change-password", changePasswordDto);
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"ChangePassword Response Status: {response.StatusCode}");
    Console.WriteLine($"ChangePassword Response Content: {content}");

    if (!response.IsSuccessStatusCode)
    {
      return ApiResponse<bool>.Fail($"API call failed with status {response.StatusCode}: {content}");
    }

    try
    {
      var result = JsonSerializer.Deserialize<ApiResponse<bool>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      return result ?? ApiResponse<bool>.Fail("Failed to change password.");
    }
    catch (JsonException ex)
    {
      Console.WriteLine($"ChangePassword Deserialize Error: {ex.Message}");
      return ApiResponse<bool>.Fail($"Failed to parse response: {ex.Message}. Content: {content}");
    }
  }

  public async Task<ApiResponse<CustomerDto>> UploadAvatarAsync(IFormFile file)
  {
    if (file == null || file.Length == 0)
    {
      Console.WriteLine("UploadAvatar failed: No file provided");
      return ApiResponse<CustomerDto>.Fail("No file uploaded.");
    }

    // Determine media type based on file extension
    string mediaType = file.FileName.ToLower().EndsWith(".png") ? "image/png" : "image/jpeg";
    Console.WriteLine($"Sending POST api/users/upload-avatar with file: {file.FileName}, mediaType: {mediaType}");
    await AddAuthorizationHeaderAsync();
    using var content = new MultipartFormDataContent();
    using var stream = file.OpenReadStream();
    var streamContent = new StreamContent(stream);
    streamContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
    content.Add(streamContent, "file", file.FileName);

    var response = await _httpClient.PostAsync("api/users/upload-avatar", content);
    var contentStr = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"UploadAvatar Response Status: {response.StatusCode}");
    Console.WriteLine($"UploadAvatar Response Content: {contentStr}");

    if (!response.IsSuccessStatusCode)
    {
      return ApiResponse<CustomerDto>.Fail($"API call failed with status {response.StatusCode}: {contentStr}");
    }

    try
    {
      var result = JsonSerializer.Deserialize<ApiResponse<CustomerDto>>(contentStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      return result ?? ApiResponse<CustomerDto>.Fail("Failed to upload avatar.");
    }
    catch (JsonException ex)
    {
      Console.WriteLine($"UploadAvatar Deserialize Error: {ex.Message}");
      return ApiResponse<CustomerDto>.Fail($"Failed to parse response: {ex.Message}. Content: {contentStr}");
    }
  }
}
