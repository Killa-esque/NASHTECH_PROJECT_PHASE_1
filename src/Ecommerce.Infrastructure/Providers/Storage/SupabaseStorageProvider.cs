using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ecommerce.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Infrastructure.Providers.Storage;

public class SupabaseStorageProvider : ISupabaseStorageService
{
  private readonly HttpClient _httpClient;
  private readonly string _bucketName = "product-images";
  private readonly string _supabaseUrl = "https://qztbgbjkjylxrxratybn.supabase.co";
  private readonly string _supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InF6dGJnYmpranlseHJ4cmF0eWJuIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc0NTM5MTU3MSwiZXhwIjoyMDYwOTY3NTcxfQ.yDZEbLd4W48cxsxJInjD-vtbDy66YFI5DZpnaRr9Jas";
  private readonly ILogger<SupabaseStorageProvider> _logger;

  public SupabaseStorageProvider(HttpClient httpClient, ILogger<SupabaseStorageProvider> logger)
  {
    _logger = logger;
    _httpClient = httpClient;
    _httpClient.BaseAddress = new Uri(_supabaseUrl);
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
  }

  public async Task<string> UploadAvatarImageAsync(Stream fileStream, string fileName, string contentType, string userId)
  {
    try
    {
      _logger.LogInformation("Uploading avatar for user: {UserId}, file: {FileName}", userId, fileName);

      // Xóa avatar cũ (nếu có)
      await DeleteOldAvatarsAsync(userId);

      // Tạo tên file duy nhất
      var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";
      var uploadUrl = $"/storage/v1/object/{_bucketName}/Avatar/{userId}/{uniqueFileName}";

      var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
      request.Content = new StreamContent(fileStream);
      request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
      request.Headers.Add("x-upsert", "true");

      var response = await _httpClient.SendAsync(request);
      var responseBody = await response.Content.ReadAsStringAsync();

      _logger.LogInformation("Supabase upload response: Status={StatusCode}, Content={Content}",
          response.StatusCode, responseBody);

      if (!response.IsSuccessStatusCode)
      {
        _logger.LogWarning("Failed to upload avatar: Status={StatusCode}, Content={Content}",
            response.StatusCode, responseBody);
        throw new Exception($"Failed to upload avatar: {response.StatusCode}, {responseBody}");
      }

      var publicUrl = $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/Avatar/{userId}/{uniqueFileName}";
      return publicUrl;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error uploading avatar for user {UserId}: {Message}", userId, ex.Message);
      throw;
    }
  }


  private async Task DeleteOldAvatarsAsync(string userId)
  {
    try
    {
      _logger.LogInformation("Listing avatars for user: {UserId}", userId);
      // Liệt kê tất cả file trong folder Avatar/{userId}
      var listRequest = new HttpRequestMessage(HttpMethod.Post, $"/storage/v1/object/list/{_bucketName}");
      listRequest.Content = new StringContent(JsonSerializer.Serialize(new { prefix = $"Avatar/{userId}/" }), Encoding.UTF8, "application/json");

      var listResponse = await _httpClient.SendAsync(listRequest);
      var listResponseBody = await listResponse.Content.ReadAsStringAsync();
      _logger.LogInformation("Supabase list response: Status={StatusCode}, Content={Content}", listResponse.StatusCode, listResponseBody);

      if (!listResponse.IsSuccessStatusCode)
      {
        _logger.LogWarning("Failed to list avatars: Status={StatusCode}, Content={Content}", listResponse.StatusCode, listResponseBody);
        throw new Exception($"Failed to list avatars: {listResponse.StatusCode}, {listResponseBody}");
      }

      var files = JsonSerializer.Deserialize<List<SupabaseFile>>(listResponseBody, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });

      if (files == null || !files.Any())
      {
        _logger.LogInformation("No old avatars found for user: {UserId}", userId);
        return;
      }

      // Lọc bỏ file .emptyFolderPlaceholder và file không hợp lệ
      var validFiles = files.Where(f => !string.IsNullOrEmpty(f.Name) && f.Name != ".emptyFolderPlaceholder").ToList();
      if (!validFiles.Any())
      {
        _logger.LogInformation("No valid avatars to delete for user: {UserId}", userId);
        return;
      }

      // Log danh sách file để debug
      _logger.LogInformation("Found {Count} valid avatars for user {UserId}: {Files}", validFiles.Count, userId, string.Join(", ", validFiles.Select(f => f.Name)));

      // Xóa từng file cũ
      var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/storage/v1/object/{_bucketName}");
      var filePaths = new { prefixes = validFiles.Select(f => $"Avatar/{userId}/{f.Name}").ToList() };
      _logger.LogInformation("Deleting avatars for user {UserId}: {Prefixes}", userId, string.Join(", ", filePaths.prefixes));

      deleteRequest.Content = new StringContent(JsonSerializer.Serialize(filePaths), Encoding.UTF8, "application/json");

      var deleteResponse = await _httpClient.SendAsync(deleteRequest);
      var deleteResponseBody = await deleteResponse.Content.ReadAsStringAsync();
      _logger.LogInformation("Supabase delete response: Status={StatusCode}, Content={Content}", deleteResponse.StatusCode, deleteResponseBody);

      if (!deleteResponse.IsSuccessStatusCode)
      {
        // Bỏ qua lỗi 404 (file không tồn tại)
        if (deleteResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
          _logger.LogWarning("Some avatars not found for deletion: Status={StatusCode}, Content={Content}", deleteResponse.StatusCode, deleteResponseBody);
          return;
        }
        _logger.LogWarning("Failed to delete old avatars: Status={StatusCode}, Content={Content}", deleteResponse.StatusCode, deleteResponseBody);
        throw new Exception($"Failed to delete old avatars: {deleteResponse.StatusCode}, {deleteResponseBody}");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting old avatars for user {UserId}: {Message}", userId, ex.Message);
      throw;
    }
  }

  public async Task<string> UploadProductImageAsync(Stream fileStream, string fileName, string contentType, Guid productId)
  {
    var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";
    var request = new HttpRequestMessage(HttpMethod.Post, $"/storage/v1/object/{_bucketName}/Product/{productId}/{uniqueFileName}");
    request.Content = new StreamContent(fileStream);
    request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
    request.Headers.Add("x-upsert", "true");

    var response = await _httpClient.SendAsync(request);
    var responseBody = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
      throw new Exception($"Failed to upload image: {response.StatusCode}, {responseBody}");

    return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/Product/{productId}/{uniqueFileName}";
  }

  public async Task<List<string>> UploadProductImagesAsync(List<(Stream fileStream, string fileName, string contentType)> files, Guid productId)
  {
    var imageUrls = new List<string>();
    foreach (var file in files)
    {
      var imageUrl = await UploadProductImageAsync(file.fileStream, file.fileName, file.contentType, productId);
      imageUrls.Add(imageUrl);
    }
    return imageUrls;
  }
}

public class SupabaseFile
{
  public string Name { get; set; }
  public string Id { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public DateTime LastAccessedAt { get; set; }
  public Dictionary<string, object> Metadata { get; set; }
}
