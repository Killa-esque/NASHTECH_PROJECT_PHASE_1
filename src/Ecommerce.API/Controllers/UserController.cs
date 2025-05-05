using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Ecommerce.Application.Interfaces.Services;
using System.Text.Json;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
  private readonly HttpClient _httpClient;
  private readonly ISupabaseStorageService _storageService;
  private readonly ILogger<UserController> _logger;

  public UserController(IHttpClientFactory httpClientFactory, ISupabaseStorageService storageService, ILogger<UserController> logger)
  {
    _logger = logger;
    _httpClient = httpClientFactory.CreateClient("AuthServerClient");
    _storageService = storageService;
  }

  private async Task AddAuthorizationHeaderAsync()
  {
    var accessToken = await HttpContext.GetTokenAsync("access_token");
    if (!string.IsNullOrEmpty(accessToken))
    {
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    // Tạm thời thêm userId vào header
    var userId = HttpContext.User.FindFirst("sub")?.Value ?? "a2ad953e-1482-4cf2-9b94-41c88aeb90e3";
    _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
  }

  [HttpPost("upload-avatar")]
  public async Task<IActionResult> UploadAvatar(IFormFile file)
  {
    if (file == null || file.Length == 0)
    {
      _logger.LogWarning("No file uploaded for avatar");
      return BadRequest(ApiResponse<string>.Fail("No file uploaded."));
    }

    var userId = HttpContext.User.FindFirst("sub")?.Value ?? "a2ad953e-1482-4cf2-9b94-41c88aeb90e3";
    _logger.LogInformation("Uploading avatar for user: {UserId}", userId);

    try
    {
      // Upload avatar lên Supabase
      using var stream = file.OpenReadStream();
      var avatarUrl = await _storageService.UploadAvatarImageAsync(stream, file.FileName, file.ContentType, userId);
      _logger.LogInformation("Avatar uploaded to Supabase: {AvatarUrl}", avatarUrl);

      // Cập nhật profile với URL avatar mới
      await AddAuthorizationHeaderAsync();
      var updateDto = new UpdateAvatarDto
      {
        AvatarUrl = avatarUrl
      };
      var response = await _httpClient.PutAsJsonAsync("api/users/profile/avatar", updateDto);
      var responseContent = await response.Content.ReadAsStringAsync();
      _logger.LogInformation("AuthorizationServer response: Status={StatusCode}, Content={Content}", response.StatusCode, responseContent);

      if (!response.IsSuccessStatusCode)
      {
        _logger.LogWarning("Failed to update profile in AuthorizationServer: Status={StatusCode}, Content={Content}", response.StatusCode, responseContent);
        return StatusCode((int)response.StatusCode, ApiResponse<string>.Fail($"Failed to update profile: {responseContent}"));
      }

      var result = JsonSerializer.Deserialize<ApiResponse<CustomerDto>>(responseContent, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });

      return result?.Status == true
          ? Ok(result)
          : BadRequest(result ?? ApiResponse<CustomerDto>.Fail("Failed to update profile."));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error uploading avatar for user {UserId}: {Message}", userId, ex.Message);
      return BadRequest(ApiResponse<string>.Fail($"Failed to upload avatar: {ex.Message}"));
    }
  }

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.GetAsync($"api/users?pageIndex={pageIndex}&pageSize={pageSize}");
    var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<CustomerDto>>>();
    return response.IsSuccessStatusCode && result?.Status == true ? Ok(result) : StatusCode((int)response.StatusCode, result);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(string id)
  {
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.GetAsync($"api/users/{id}");
    var result = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();
    return response.IsSuccessStatusCode && result?.Status == true ? Ok(result) : StatusCode((int)response.StatusCode, result);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateCustomerDto createDto)
  {
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.PostAsJsonAsync("api/users", createDto);
    var result = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();
    return response.IsSuccessStatusCode && result?.Status == true ? CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result) : StatusCode((int)response.StatusCode, result);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(string id, [FromBody] UpdateCustomerDto updateDto)
  {
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.PutAsJsonAsync($"api/users/{id}", updateDto);
    var result = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();
    return response.IsSuccessStatusCode && result?.Status == true ? Ok(result) : StatusCode((int)response.StatusCode, result);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(string id)
  {
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.DeleteAsync($"api/users/{id}");
    var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    return response.IsSuccessStatusCode && result?.Status == true ? Ok(result) : StatusCode((int)response.StatusCode, result);
  }

  [HttpGet("profile")]
  public async Task<IActionResult> GetProfile()
  {
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.GetAsync("api/users/profile");
    var result = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();
    return response.IsSuccessStatusCode && result?.Status == true ? Ok(result) : StatusCode((int)response.StatusCode, result);
  }

  [HttpPut("profile")]
  public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerDto updateDto)
  {
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.PutAsJsonAsync("api/users/profile", updateDto);
    var result = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();
    return response.IsSuccessStatusCode && result?.Status == true ? Ok(result) : StatusCode((int)response.StatusCode, result);
  }

  [HttpPost("change-password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
  {
    await AddAuthorizationHeaderAsync();
    var response = await _httpClient.PostAsJsonAsync("api/users/change-password", changePasswordDto);
    var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    return response.IsSuccessStatusCode && result?.Status == true ? Ok(result) : StatusCode((int)response.StatusCode, result);
  }
}
