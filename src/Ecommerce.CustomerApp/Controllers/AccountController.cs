using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ecommerce.CustomerApp.Controllers;

[Authorize]
public class AccountController : Controller
{
  private readonly IUserService _userService;
  private readonly ILogger<AccountController> _logger;

  public AccountController(IUserService userService, ILogger<AccountController> logger)
  {
    _userService = userService;
    _logger = logger;
  }

  // GET: Render view Profile
  [HttpGet]
  public IActionResult Profile()
  {
    return View();
  }

  // GET: Lấy thông tin profile cho AJAX
  [HttpGet]
  public async Task<IActionResult> GetProfile()
  {
    try
    {
      _logger.LogInformation("Fetching profile");
      var profileResult = await _userService.GetProfileAsync();
      if (!profileResult.Status)
      {
        _logger.LogWarning("Failed to fetch profile: {Message}", profileResult.Message);
        return Json(new { success = false, message = profileResult.Message });
      }

      var data = new
      {
        fullName = profileResult.Data.FullName,
        email = profileResult.Data.Email,
        phoneNumber = profileResult.Data.PhoneNumber,
        dateOfBirth = profileResult.Data.DateOfBirth?.ToString("dd-MM-yyyy"),
        gender = profileResult.Data.Gender,
        defaultAddress = profileResult.Data.DefaultAddress,
        allergyNotes = profileResult.Data.AllergyNotes,
        avatarUrl = profileResult.Data.AvatarUrl
      };

      return Json(new { success = true, data });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching profile");
      return Json(new { success = false, message = "Không thể tải thông tin hồ sơ." });
    }
  }

  // POST: Cập nhật profile
  [HttpPost]
  public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateViewModel model)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        _logger.LogWarning("Invalid profile input: {Errors}", string.Join(", ", errors));
        return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors });
      }

      _logger.LogInformation("Updating profile");
      var profileResult = await _userService.UpdateProfileAsync(model);
      if (!profileResult.Status)
      {
        _logger.LogWarning("Failed to update profile: {Message}", profileResult.Message);
        return Json(new { success = false, message = profileResult.Message });
      }

      var data = new
      {
        fullName = profileResult.Data.FullName,
        email = profileResult.Data.Email,
        phoneNumber = profileResult.Data.PhoneNumber,
        dateOfBirth = profileResult.Data.DateOfBirth?.ToString("dd-MM-yyyy"),
        gender = profileResult.Data.Gender,
        defaultAddress = profileResult.Data.DefaultAddress,
        allergyNotes = profileResult.Data.AllergyNotes,
        avatarUrl = profileResult.Data.AvatarUrl
      };

      return Json(new { success = true, message = "Cập nhật hồ sơ thành công.", data });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating profile");
      return Json(new { success = false, message = "Không thể cập nhật hồ sơ." });
    }
  }

  // POST: Đổi mật khẩu
  [HttpPost]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        _logger.LogWarning("Invalid change password input: {Errors}", string.Join(", ", errors));
        return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors });
      }

      _logger.LogInformation("Changing password");
      var passwordResult = await _userService.ChangePasswordAsync(model);
      if (!passwordResult.Status)
      {
        _logger.LogWarning("Failed to change password: {Message}", passwordResult.Message);
        return Json(new { success = false, message = passwordResult.Message });
      }

      return Json(new { success = true, message = "Đổi mật khẩu thành công." });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error changing password");
      return Json(new { success = false, message = "Không thể đổi mật khẩu." });
    }
  }

  // POST: Upload avatar
  [HttpPost]
  public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
  {
    try
    {
      if (avatarFile == null || avatarFile.Length == 0)
      {
        _logger.LogWarning("No file uploaded for avatar");
        return Json(new { success = false, message = "Không có file được tải lên." });
      }

      _logger.LogInformation("Uploading avatar");
      var avatarResult = await _userService.UploadAvatarAsync(avatarFile);
      if (!avatarResult.Status)
      {
        _logger.LogWarning("Failed to upload avatar: {Message}", avatarResult.Message);
        return Json(new { success = false, message = avatarResult.Message });
      }

      var data = new
      {
        avatarUrl = avatarResult.Data.AvatarUrl
      };

      return Json(new { success = true, message = "Tải lên avatar thành công.", data });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error uploading avatar");
      return Json(new { success = false, message = "Không thể tải lên avatar." });
    }
  }
}
