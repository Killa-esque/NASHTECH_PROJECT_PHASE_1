using System.Net.Http.Headers;
using Ecommerce.Application.Interfaces.Services;

namespace Ecommerce.Infrastructure.Providers.Storage;

public class SupabaseStorageProvider : ISupabaseStorageService
{
  private readonly HttpClient _httpClient;
  private readonly string _bucketName = "product-images";
  private readonly string _supabaseUrl = "https://qztbgbjkjylxrxratybn.supabase.co";
  private readonly string _supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InF6dGJnYmpranlseHJ4cmF0eWJuIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc0NTM5MTU3MSwiZXhwIjoyMDYwOTY3NTcxfQ.yDZEbLd4W48cxsxJInjD-vtbDy66YFI5DZpnaRr9Jas";

  public SupabaseStorageProvider(HttpClient httpClient)
  {
    _httpClient = httpClient;
    _httpClient.BaseAddress = new Uri(_supabaseUrl);
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
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

    Console.WriteLine($"Response: {responseBody}");
    Console.WriteLine($"Status Code: {response.StatusCode}");

    if (!response.IsSuccessStatusCode)
      throw new Exception($"Failed to upload image: {response.StatusCode}, {responseBody}");

    return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/Product/{productId}/{uniqueFileName}";
  }

  public async Task<string> UploadAvatarImageAsync(Stream fileStream, string fileName, string contentType, Guid userId)
  {
    var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";

    var request = new HttpRequestMessage(HttpMethod.Post, $"/storage/v1/object/{_bucketName}/Avatar/{userId}/{uniqueFileName}");
    request.Content = new StreamContent(fileStream);
    request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
    request.Headers.Add("x-upsert", "true");

    var response = await _httpClient.SendAsync(request);
    var responseBody = await response.Content.ReadAsStringAsync();

    Console.WriteLine($"Response: {responseBody}");
    Console.WriteLine($"Status Code: {response.StatusCode}");

    if (!response.IsSuccessStatusCode)
      throw new Exception($"Failed to upload avatar: {response.StatusCode}, {responseBody}");

    return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/Avatar/{userId}/{uniqueFileName}";
  }


}
