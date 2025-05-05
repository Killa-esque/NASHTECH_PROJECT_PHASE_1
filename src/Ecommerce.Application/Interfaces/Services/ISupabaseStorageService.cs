namespace Ecommerce.Application.Interfaces.Services;
public interface ISupabaseStorageService
{
  Task<string> UploadProductImageAsync(Stream fileStream, string fileName, string contentType, Guid productId);
  Task<List<string>> UploadProductImagesAsync(List<(Stream fileStream, string fileName, string contentType)> files, Guid productId);
  Task<string> UploadAvatarImageAsync(Stream fileStream, string fileName, string contentType, string userId);
}
