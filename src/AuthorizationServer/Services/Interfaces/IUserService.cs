using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;

namespace AuthorizationServer.Services.Intefaces;

public interface IUserService
{
  Task<ApiResponse<PagedResult<CustomerDto>>> GetAllAsync(int pageIndex, int pageSize);
  Task<ApiResponse<CustomerDto>> GetByIdAsync(string id);
  Task<ApiResponse<CustomerDto>> CreateAsync(CreateCustomerDto createDto);
  Task<ApiResponse<CustomerDto>> UpdateAsync(string id, UpdateCustomerDto updateDto);
  Task<ApiResponse<bool>> DeleteAsync(string id);
  Task<ApiResponse<CustomerDto>> GetProfileAsync(string userId);
  Task<ApiResponse<CustomerDto>> UpdateProfileAsync(string userId, UpdateCustomerDto updateDto);
  Task<ApiResponse<CustomerDto>> UpdateProfileAsync(string userId, UpdateAvatarDto updateDto);
  Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
}
