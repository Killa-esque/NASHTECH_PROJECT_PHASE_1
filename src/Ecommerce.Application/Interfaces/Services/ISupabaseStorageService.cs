namespace Ecommerce.Application.Interfaces.Services;
public interface ISupabaseStorageService
{
  Task<List<string>> UploadProductImagesAsync(List<(Stream fileStream, string fileName, string contentType)> files, Guid productId);
  Task<string> UploadAvatarImageAsync(Stream fileStream, string fileName, string contentType, string userId);
  Task<bool> DeleteProductImageAsync(Guid productId, string imageFileName);
}
