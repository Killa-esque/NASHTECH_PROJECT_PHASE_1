using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services.Interfaces;

public interface IUserService
{
  Task<ApiResponse<CustomerDto>> GetProfileAsync();
  Task<ApiResponse<CustomerDto>> UpdateProfileAsync(ProfileUpdateViewModel model);
  Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordViewModel model);
  Task<ApiResponse<CustomerDto>> UploadAvatarAsync(IFormFile file);
}
