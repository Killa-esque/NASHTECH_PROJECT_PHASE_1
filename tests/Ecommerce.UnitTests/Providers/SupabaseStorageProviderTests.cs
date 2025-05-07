using System.Net;
using System.Text;
using System.Text.Json;
using Ecommerce.Infrastructure.Providers.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Ecommerce.Unitest.Providers
{
  public class SupabaseStorageProviderTests
  {
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<ILogger<SupabaseStorageProvider>> _mockLogger;
    private readonly HttpClient _httpClient;
    private readonly SupabaseStorageProvider _provider;
    private readonly string _supabaseUrl = "https://qztbgbjkjylxrxratybn.supabase.co";
    private readonly string _bucketName = "product-images";

    public SupabaseStorageProviderTests()
    {
      _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
      _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
      _mockLogger = new Mock<ILogger<SupabaseStorageProvider>>();
      _provider = new SupabaseStorageProvider(_httpClient, _mockLogger.Object);
    }

    [Fact]
    public async Task UploadProductImagesAsync_Success_ReturnsImageUrls()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var files = new List<(Stream fileStream, string fileName, string contentType)>
            {
                (new MemoryStream(Encoding.UTF8.GetBytes("test")), "test.jpg", "image/jpeg")
            };
      var expectedPath = $"/storage/v1/object/public/{_bucketName}/Product/{productId}/";

      _mockHttpMessageHandler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post &&
                  req.RequestUri.ToString().Contains($"/storage/v1/object/{_bucketName}/Product/{productId}/")),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"message\":\"Upload successful\"}")
          });

      // Act
      var result = await _provider.UploadProductImagesAsync(files, productId);

      // Assert
      Assert.Single(result);
      Assert.Contains(expectedPath, result[0]);
      Assert.EndsWith("-test.jpg", result[0]);
    }

    [Fact]
    public async Task UploadProductImagesAsync_Failure_ThrowsException()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var files = new List<(Stream fileStream, string fileName, string contentType)>
            {
                (new MemoryStream(Encoding.UTF8.GetBytes("test")), "test.jpg", "image/jpeg")
            };

      _mockHttpMessageHandler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("{\"error\":\"Upload failed\"}")
          });

      // Act & Assert
      await Assert.ThrowsAsync<Exception>(() => _provider.UploadProductImagesAsync(files, productId));
    }

    [Fact]
    public async Task UploadAvatarImageAsync_Success_ReturnsPublicUrl()
    {
      // Arrange
      var userId = "a2ad953e-1482-4cf2-9b94-41c88aeb90e3";
      var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
      var fileName = "avatar.jpg";
      var contentType = "image/jpeg";
      var expectedPath = $"/storage/v1/object/public/{_bucketName}/Avatar/{userId}/";

      // Mock list old avatars (empty list)
      _mockHttpMessageHandler.Protected()
          .SetupSequence<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post &&
                  req.RequestUri.ToString().Contains($"/storage/v1/object/list/{_bucketName}")),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("[]")
          });

      // Mock upload new avatar
      _mockHttpMessageHandler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post &&
                  req.RequestUri.ToString().Contains($"/storage/v1/object/{_bucketName}/Avatar/{userId}/")),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"message\":\"Upload successful\"}")
          });

      // Act
      var result = await _provider.UploadAvatarImageAsync(fileStream, fileName, contentType, userId);

      // Assert
      Assert.Contains(expectedPath, result);
      Assert.EndsWith("-avatar.jpg", result);
    }

    [Fact]
    public async Task UploadAvatarImageAsync_Failure_ThrowsException()
    {
      // Arrange
      var userId = "a2ad953e-1482-4cf2-9b94-41c88aeb90e3";
      var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
      var fileName = "avatar.jpg";
      var contentType = "image/jpeg";

      // Mock list old avatars
      _mockHttpMessageHandler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post &&
                  req.RequestUri.ToString().Contains($"/storage/v1/object/list/{_bucketName}")),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("[]")
          });

      // Mock upload failure
      _mockHttpMessageHandler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post &&
                  req.RequestUri.ToString().Contains($"/storage/v1/object/{_bucketName}/Avatar/{userId}/")),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("{\"error\":\"Upload failed\"}")
          });

      // Act & Assert
      await Assert.ThrowsAsync<Exception>(() => _provider.UploadAvatarImageAsync(fileStream, fileName, contentType, userId));
    }

    [Fact]
    public async Task UploadAvatarImageAsync_DeletesOldAvatars_Success()
    {
      // Arrange
      var userId = "a2ad953e-1482-4cf2-9b94-41c88aeb90e3";
      var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
      var fileName = "avatar.jpg";
      var contentType = "image/jpeg";
      var expectedPath = $"/storage/v1/object/public/{_bucketName}/Avatar/{userId}/";

      // Mock list old avatars
      var oldFiles = new List<SupabaseFile>
            {
                new SupabaseFile { Name = "old-avatar.jpg" }
            };
      _mockHttpMessageHandler.Protected()
          .SetupSequence<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post &&
                  req.RequestUri.ToString().Contains($"/storage/v1/object/list/{_bucketName}")),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(oldFiles))
          });

      // Mock delete old avatars
      _mockHttpMessageHandler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Delete &&
                  req.RequestUri.ToString().Contains($"/storage/v1/object/{_bucketName}")),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"message\":\"Deleted\"}")
          });

      // Mock upload new avatar
      _mockHttpMessageHandler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post &&
                  req.RequestUri.ToString().Contains($"/storage/v1/object/{_bucketName}/Avatar/{userId}/")),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"message\":\"Upload successful\"}")
          });

      // Act
      var result = await _provider.UploadAvatarImageAsync(fileStream, fileName, contentType, userId);

      // Assert
      Assert.Contains(expectedPath, result);
      Assert.EndsWith("-avatar.jpg", result);
      _mockHttpMessageHandler.Protected().Verify(
          "SendAsync",
          Times.Once(),
          ItExpr.Is<HttpRequestMessage>(req =>
              req.Method == HttpMethod.Delete &&
              req.RequestUri.ToString().Contains($"/storage/v1/object/{_bucketName}")),
          ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task DeleteProductImageAsync_Success_ReturnsTrue()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var imageFileName = "test.jpg";

      _mockHttpMessageHandler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Delete &&
                  req.RequestUri.ToString().Contains($"/storage/v1/object/{_bucketName}/Product/{productId}/{imageFileName}")),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"message\":\"Deleted\"}")
          });

      // Act
      var result = await _provider.DeleteProductImageAsync(productId, imageFileName);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public async Task DeleteProductImageAsync_Failure_ReturnsFalse()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var imageFileName = "test.jpg";

      _mockHttpMessageHandler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Delete &&
                  req.RequestUri.ToString().Contains($"/storage/v1/object/{_bucketName}/Product/{productId}/{imageFileName}")),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("{\"error\":\"Not found\"}")
          });

      // Act
      var result = await _provider.DeleteProductImageAsync(productId, imageFileName);

      // Assert
      Assert.False(result);
    }
  }
}
