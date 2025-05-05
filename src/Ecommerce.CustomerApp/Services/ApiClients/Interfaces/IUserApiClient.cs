using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;

namespace Ecommerce.CustomerApp.Services.ApiClients.Interfaces;

public interface IUserApiClient
{
  Task<ApiResponse<CustomerDto>> GetProfileAsync();
  Task<ApiResponse<CustomerDto>> UpdateProfileAsync(UpdateCustomerDto updateDto);
  Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
  Task<ApiResponse<CustomerDto>> UploadAvatarAsync(IFormFile file);
}
