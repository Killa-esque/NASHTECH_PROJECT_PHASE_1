using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.Shared.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.CustomerApp.Services.ApiClients;

public class UploadApiClient : IUploadApiClient
{
  private readonly HttpClient _httpClient;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public UploadApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
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

  // public async Task<string> UploadAvatarAsync(string userId, IFormFile file)
  // {
  //   await AddAuthorizationHeaderAsync();
  //   using var content = new MultipartFormDataContent();
  //   using var stream = file.OpenReadStream();
  //   content.Add(new StreamContent(stream), "file", file.FileName);
  //   var response = await _httpClient.PostAsync($"/api/admin/images/upload-avatar/{userId}", content);
  //   response.EnsureSuccessStatusCode();
  //   var result = JsonSerializer.Deserialize<ApiResponse<object>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
  //   return result!.Data.avatarUrl.ToString();
  // }
}
