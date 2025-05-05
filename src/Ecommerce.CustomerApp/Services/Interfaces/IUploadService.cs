namespace Ecommerce.CustomerApp.Services.Interfaces;

public interface IUploadService
{
  Task<string> UploadAvatarAsync(string userId, IFormFile file);
}
