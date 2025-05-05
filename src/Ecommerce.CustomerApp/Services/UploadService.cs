using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.CustomerApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.CustomerApp.Services;

public class UploadService : IUploadService
{
  private readonly IUploadApiClient _uploadApiClient;

  public UploadService(IUploadApiClient uploadApiClient)
  {
    _uploadApiClient = uploadApiClient;
  }

  public async Task<string> UploadAvatarAsync(string userId, IFormFile file)
  {
    // return await _uploadApiClient.UploadAvatarAsync(userId, file);
    return string.Empty; // Chưa implement UploadAvatarAsync trong UploadApiClient
  }
}
