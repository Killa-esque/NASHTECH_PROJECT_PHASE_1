using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services;

public class UserService : IUserService
{
  private readonly IUserApiClient _userApiClient;

  public UserService(IUserApiClient userApiClient)
  {
    _userApiClient = userApiClient;
  }

  public async Task<ApiResponse<CustomerDto>> GetProfileAsync()
  {
    return await _userApiClient.GetProfileAsync();
  }

  public async Task<ApiResponse<CustomerDto>> UpdateProfileAsync(ProfileUpdateViewModel model)
  {
    var updateDto = new UpdateCustomerDto
    {
      FullName = model.FullName,
      DateOfBirth = model.DateOfBirth,
      Gender = model.Gender,
      DefaultAddress = model.DefaultAddress,
      AllergyNotes = model.AllergyNotes,
      PhoneNumber = model.PhoneNumber
    };
    return await _userApiClient.UpdateProfileAsync(updateDto);
  }

  public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordViewModel model)
  {
    var changePasswordDto = new ChangePasswordDto
    {
      CurrentPassword = model.CurrentPassword,
      NewPassword = model.NewPassword
    };
    return await _userApiClient.ChangePasswordAsync(changePasswordDto);
  }

  public async Task<ApiResponse<CustomerDto>> UploadAvatarAsync(IFormFile file)
  {
    return await _userApiClient.UploadAvatarAsync(file);
  }
}
