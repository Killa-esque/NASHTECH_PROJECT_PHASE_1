using AutoMapper;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using AuthorizationServer.Services.Intefaces;

namespace AuthorizationServer.Services;

public class UserService : IUserService
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IMapper _mapper;
  private readonly IMemoryCache _cache;
  private readonly ILogger<UserService> _logger;

  public UserService(
      UserManager<ApplicationUser> userManager,
      IMapper mapper,
      IMemoryCache cache,
      ILogger<UserService> logger)
  {
    _userManager = userManager;
    _mapper = mapper;
    _cache = cache;
    _logger = logger;
  }

  public async Task<ApiResponse<PagedResult<CustomerDto>>> GetAllAsync(int pageIndex, int pageSize)
  {
    try
    {
      _logger.LogInformation("Retrieving users: pageIndex={PageIndex}, pageSize={PageSize}", pageIndex, pageSize);
      if (pageIndex < 1 || pageSize <= 0)
      {
        _logger.LogWarning("Invalid paging parameters: pageIndex={PageIndex}, pageSize={PageSize}", pageIndex, pageSize);
        return ApiResponse<PagedResult<CustomerDto>>.Fail("Invalid paging parameters.");
      }

      var cacheKey = $"Users_{pageIndex}_{pageSize}";
      if (!_cache.TryGetValue(cacheKey, out PagedResult<CustomerDto> pagedResult))
      {
        var users = await _userManager.Users
            .AsNoTracking()
            .OrderBy(u => u.UserName)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(users);
        var totalCount = await _userManager.Users.CountAsync();

        pagedResult = PagedResult<CustomerDto>.Create(customerDtos, totalCount, pageIndex, pageSize);
        _cache.Set(cacheKey, pagedResult, TimeSpan.FromMinutes(5));
        _logger.LogInformation("Cached users: cacheKey={CacheKey}", cacheKey);
      }

      return ApiResponse<PagedResult<CustomerDto>>.Success(pagedResult, "Users retrieved successfully.");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving users: pageIndex={PageIndex}, pageSize={PageSize}", pageIndex, pageSize);
      return ApiResponse<PagedResult<CustomerDto>>.Fail("An error occurred while retrieving users.");
    }
  }

  public async Task<ApiResponse<CustomerDto>> GetByIdAsync(string id)
  {
    try
    {
      _logger.LogInformation("Retrieving user: {Id}", id);
      if (string.IsNullOrEmpty(id))
      {
        _logger.LogWarning("User ID is required: {Id}", id);
        return ApiResponse<CustomerDto>.Fail("User ID is required.");
      }

      var user = await _userManager.FindByIdAsync(id);
      if (user == null)
      {
        _logger.LogWarning("User not found: {Id}", id);
        return ApiResponse<CustomerDto>.Fail("User not found.");
      }

      var customerDto = _mapper.Map<CustomerDto>(user);
      return ApiResponse<CustomerDto>.Success(customerDto, "User retrieved successfully.");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving user: {Id}", id);
      return ApiResponse<CustomerDto>.Fail("An error occurred while retrieving user.");
    }
  }

  public async Task<ApiResponse<CustomerDto>> CreateAsync(CreateCustomerDto createDto)
  {
    try
    {
      _logger.LogInformation("Creating user: {Email}", createDto.Email);
      var user = new ApplicationUser
      {
        UserName = createDto.Email,
        Email = createDto.Email,
        FullName = createDto.FullName,
        Gender = createDto.Gender,
        DefaultAddress = createDto.DefaultAddress,
        AvatarUrl = String.Empty,
        AllergyNotes = createDto.AllergyNotes
      };

      if (!string.IsNullOrEmpty(createDto.DateOfBirth))
      {
        if (!DateTime.TryParseExact(createDto.DateOfBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth))
        {
          _logger.LogWarning("Invalid DateOfBirth format: {DateOfBirth}", createDto.DateOfBirth);
          return ApiResponse<CustomerDto>.Fail("DateOfBirth must be in format dd-MM-yyyy.");
        }
        user.DateOfBirth = DateTime.SpecifyKind(dateOfBirth.Date, DateTimeKind.Utc);
      }

      var result = await _userManager.CreateAsync(user, createDto.Password);
      if (!result.Succeeded)
      {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        _logger.LogWarning("Failed to create user: {Errors}", errors);
        return ApiResponse<CustomerDto>.Fail($"Failed to create user: {errors}");
      }

      await _userManager.AddToRoleAsync(user, "customer");
      var customerDto = _mapper.Map<CustomerDto>(user);
      _logger.LogInformation("User created: {Id}", user.Id);
      return ApiResponse<CustomerDto>.Success(customerDto, "User created successfully.");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error creating user: {Email}", createDto.Email);
      return ApiResponse<CustomerDto>.Fail("An error occurred while creating user.");
    }
  }

  public async Task<ApiResponse<CustomerDto>> UpdateAsync(string id, UpdateCustomerDto updateDto)
  {
    try
    {
      _logger.LogInformation("Updating user: {Id}", id);
      if (string.IsNullOrEmpty(id))
      {
        _logger.LogWarning("User ID is required: {Id}", id);
        return ApiResponse<CustomerDto>.Fail("User ID is required.");
      }

      var user = await _userManager.FindByIdAsync(id);
      if (user == null)
      {
        _logger.LogWarning("User not found: {Id}", id);
        return ApiResponse<CustomerDto>.Fail("User not found.");
      }

      if (!string.IsNullOrEmpty(updateDto.DateOfBirth))
      {
        if (!DateTime.TryParseExact(updateDto.DateOfBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth))
        {
          _logger.LogWarning("Invalid DateOfBirth format: {DateOfBirth}", updateDto.DateOfBirth);
          return ApiResponse<CustomerDto>.Fail("DateOfBirth must be in format dd-MM-yyyy.");
        }
        user.DateOfBirth = DateTime.SpecifyKind(dateOfBirth.Date, DateTimeKind.Utc);
      }
      else
      {
        user.DateOfBirth = null;
      }

      _mapper.Map(updateDto, user);
      var result = await _userManager.UpdateAsync(user);
      if (!result.Succeeded)
      {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        _logger.LogWarning("Failed to update user: {Errors}", errors);
        return ApiResponse<CustomerDto>.Fail($"Failed to update user: {errors}");
      }

      var updatedCustomerDto = _mapper.Map<CustomerDto>(user);
      _logger.LogInformation("User updated: {Id}", id);
      return ApiResponse<CustomerDto>.Success(updatedCustomerDto, "User updated successfully.");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating user: {Id}", id);
      return ApiResponse<CustomerDto>.Fail("An error occurred while updating user.");
    }
  }

  public async Task<ApiResponse<bool>> DeleteAsync(string id)
  {
    try
    {
      _logger.LogInformation("Deleting user: {Id}", id);
      if (string.IsNullOrEmpty(id))
      {
        _logger.LogWarning("User ID is required: {Id}", id);
        return ApiResponse<bool>.Fail("User ID is required.");
      }

      var user = await _userManager.FindByIdAsync(id);
      if (user == null)
      {
        _logger.LogWarning("User not found: {Id}", id);
        return ApiResponse<bool>.Fail("User not found.");
      }

      var result = await _userManager.DeleteAsync(user);
      if (!result.Succeeded)
      {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        _logger.LogWarning("Failed to delete user: {Errors}", errors);
        return ApiResponse<bool>.Fail($"Failed to delete user: {errors}");
      }

      _logger.LogInformation("User deleted: {Id}", id);
      return ApiResponse<bool>.Success(true, "User deleted successfully.");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting user: {Id}", id);
      return ApiResponse<bool>.Fail("An error occurred while deleting user.");
    }
  }

  public async Task<ApiResponse<CustomerDto>> GetProfileAsync(string userId)
  {
    try
    {
      _logger.LogInformation("Retrieving profile for user: {UserId}", userId);
      if (string.IsNullOrEmpty(userId))
      {
        _logger.LogWarning("User ID is required: {UserId}", userId);
        return ApiResponse<CustomerDto>.Fail("User ID is required.");
      }

      var cacheKey = $"UserProfile_{userId}";
      if (!_cache.TryGetValue(cacheKey, out CustomerDto customerDto))
      {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
          _logger.LogWarning("User not found: {UserId}", userId);
          return ApiResponse<CustomerDto>.Fail("User not found.");
        }

        customerDto = _mapper.Map<CustomerDto>(user);
        _cache.Set(cacheKey, customerDto, TimeSpan.FromMinutes(10));
        _logger.LogInformation("Cached profile: cacheKey={CacheKey}", cacheKey);
      }

      return ApiResponse<CustomerDto>.Success(customerDto, "Profile retrieved successfully.");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving profile: {UserId}", userId);
      return ApiResponse<CustomerDto>.Fail("An error occurred while retrieving profile.");
    }
  }

  public async Task<ApiResponse<CustomerDto>> UpdateProfileAsync(string userId, UpdateCustomerDto updateDto)
  {
    try
    {
      _logger.LogInformation("Updating profile for user: {UserId}", userId);
      if (string.IsNullOrEmpty(userId))
      {
        _logger.LogWarning("User ID is required: {UserId}", userId);
        return ApiResponse<CustomerDto>.Fail("User ID is required.");
      }

      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        _logger.LogWarning("User not found: {UserId}", userId);
        return ApiResponse<CustomerDto>.Fail("User not found.");
      }

      if (!string.IsNullOrEmpty(updateDto.DateOfBirth))
      {
        if (!DateTime.TryParseExact(updateDto.DateOfBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth))
        {
          _logger.LogWarning("Invalid DateOfBirth format: {DateOfBirth}", updateDto.DateOfBirth);
          return ApiResponse<CustomerDto>.Fail("DateOfBirth must be in format dd-MM-yyyy.");
        }
        user.DateOfBirth = DateTime.SpecifyKind(dateOfBirth.Date, DateTimeKind.Utc);
      }
      else
      {
        user.DateOfBirth = null;
      }

      _mapper.Map(updateDto, user);
      var result = await _userManager.UpdateAsync(user);
      if (!result.Succeeded)
      {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        _logger.LogWarning("Failed to update profile: {Errors}", errors);
        return ApiResponse<CustomerDto>.Fail($"Failed to update profile: {errors}");
      }

      var updatedCustomerDto = _mapper.Map<CustomerDto>(user);
      _cache.Remove($"UserProfile_{userId}");
      _logger.LogInformation("Profile updated: {UserId}", userId);
      return ApiResponse<CustomerDto>.Success(updatedCustomerDto, "Profile updated successfully.");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating profile: {UserId}", userId);
      return ApiResponse<CustomerDto>.Fail("An error occurred while updating profile.");
    }
  }

  public async Task<ApiResponse<CustomerDto>> UpdateProfileAsync(string userId, UpdateAvatarDto updateDto)
  {
    try
    {
      _logger.LogInformation("Updating avatar for user: {UserId}", userId);
      if (string.IsNullOrEmpty(userId))
      {
        _logger.LogWarning("User ID is required: {UserId}", userId);
        return ApiResponse<CustomerDto>.Fail("User ID is required.");
      }

      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        _logger.LogWarning("User not found: {UserId}", userId);
        return ApiResponse<CustomerDto>.Fail("User not found.");
      }

      user.AvatarUrl = updateDto.AvatarUrl;
      var result = await _userManager.UpdateAsync(user);
      if (!result.Succeeded)
      {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        _logger.LogWarning("Failed to update avatar: {Errors}", errors);
        return ApiResponse<CustomerDto>.Fail($"Failed to update avatar: {errors}");
      }

      var updatedCustomerDto = _mapper.Map<CustomerDto>(user);
      _cache.Remove($"UserProfile_{userId}");
      _logger.LogInformation("Avatar updated: {UserId}", userId);
      return ApiResponse<CustomerDto>.Success(updatedCustomerDto, "Avatar updated successfully.");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating avatar: {UserId}", userId);
      return ApiResponse<CustomerDto>.Fail("An error occurred while updating avatar.");
    }
  }

  public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
  {
    try
    {
      _logger.LogInformation("Changing password for user: {UserId}", userId);
      if (string.IsNullOrEmpty(userId))
      {
        _logger.LogWarning("User ID is required: {UserId}", userId);
        return ApiResponse<bool>.Fail("User ID is required.");
      }

      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        _logger.LogWarning("User not found: {UserId}", userId);
        return ApiResponse<bool>.Fail("User not found.");
      }

      var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
      if (!result.Succeeded)
      {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        _logger.LogWarning("Failed to change password: {Errors}", errors);
        return ApiResponse<bool>.Fail($"Failed to change password: {errors}");
      }

      _logger.LogInformation("Password changed: {UserId}", userId);
      return ApiResponse<bool>.Success(true, "Password changed successfully.");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error changing password: {UserId}", userId);
      return ApiResponse<bool>.Fail("An error occurred while changing password.");
    }
  }
}
