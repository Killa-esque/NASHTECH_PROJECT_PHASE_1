using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using AuthorizationServer.Services.Intefaces;

namespace AuthorizationServer.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
  private readonly IUserService _userService;
  private readonly ILogger<UserController> _logger;

  public UserController(IUserService userService, ILogger<UserController> logger)
  {
    _userService = userService;
    _logger = logger;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    try
    {
      _logger.LogInformation("GetAll users: pageIndex={PageIndex}, pageSize={PageSize}", pageIndex, pageSize);
      var result = await _userService.GetAllAsync(pageIndex, pageSize);
      return result.Status ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving users: pageIndex={PageIndex}, pageSize={PageSize}", pageIndex, pageSize);
      return StatusCode(500, ApiResponse<string>.Fail("An error occurred while retrieving users."));
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(string id)
  {
    try
    {
      _logger.LogInformation("Get user by ID: {Id}", id);
      var result = await _userService.GetProfileAsync(id);
      return result.Status ? Ok(result) : result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving user: {Id}", id);
      return StatusCode(500, ApiResponse<string>.Fail("An error occurred while retrieving user."));
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateCustomerDto createDto)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        _logger.LogWarning("Invalid create user input: {Errors}", string.Join(", ", errors));
        return BadRequest(ApiResponse<string>.Fail("Invalid input data."));
      }

      _logger.LogInformation("Creating user: {Email}", createDto.Email);
      var result = await _userService.CreateAsync(createDto);
      return result.Status ? CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error creating user: {Email}", createDto.Email);
      return StatusCode(500, ApiResponse<string>.Fail("An error occurred while creating user."));
    }
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(string id, [FromBody] UpdateCustomerDto updateDto)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        _logger.LogWarning("Invalid update user input: {Id}, Errors: {Errors}", id, string.Join(", ", errors));
        return BadRequest(ApiResponse<string>.Fail("Invalid input data."));
      }

      _logger.LogInformation("Updating user: {Id}", id);
      var result = await _userService.UpdateAsync(id, updateDto);
      return result.Status ? Ok(result) : result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating user: {Id}", id);
      return StatusCode(500, ApiResponse<string>.Fail("An error occurred while updating user."));
    }
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(string id)
  {
    try
    {
      _logger.LogInformation("Deleting user: {Id}", id);
      var result = await _userService.DeleteAsync(id);
      return result.Status ? Ok(result) : result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting user: {Id}", id);
      return StatusCode(500, ApiResponse<string>.Fail("An error occurred while deleting user."));
    }
  }

  [HttpGet("profile")]
  public async Task<IActionResult> GetProfile()
  {
    try
    {
      var userId = GetUserId();
      _logger.LogInformation("Getting profile for user: {UserId}", userId);
      var result = await _userService.GetProfileAsync(userId);
      return result.Status ? Ok(result) : result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogWarning("Missing X-User-Id header: {Message}", ex.Message);
      return BadRequest(ApiResponse<string>.Fail(ex.Message));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving profile");
      return StatusCode(500, ApiResponse<string>.Fail("An error occurred while retrieving profile."));
    }
  }

  [HttpPut("profile")]
  public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerDto updateDto)
  {
    try
    {
      var userId = GetUserId();
      _logger.LogInformation("Updating profile for user: {UserId}", userId);

      if (!ModelState.IsValid)
      {
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        _logger.LogWarning("Invalid update profile input: Errors: {Errors}", string.Join(", ", errors));
        return BadRequest(ApiResponse<string>.Fail("Invalid input data."));
      }

      var result = await _userService.UpdateProfileAsync(userId, updateDto);
      return result.Status ? Ok(result) : result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogWarning("Missing X-User-Id header: {Message}", ex.Message);
      return BadRequest(ApiResponse<string>.Fail(ex.Message));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating profile");
      return StatusCode(500, ApiResponse<string>.Fail("An error occurred while updating profile."));
    }
  }

  [HttpPut("profile/avatar")]
  public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarDto avatarDto)
  {
    try
    {
      var userId = GetUserId();
      _logger.LogInformation("Updating avatar for user: {UserId}", userId);

      // if (!ModelState.IsValid)
      // {
      //   var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
      //   _logger.LogWarning("Invalid update avatar input: Errors: {Errors}", string.Join(", ", errors));
      //   return BadRequest(ApiResponse<string>.Fail("Invalid input data."));
      // }

      // Log the avatar URL for debugging
      _logger.LogInformation("Avatar URL: {AvatarUrl}", avatarDto.AvatarUrl);

      var result = await _userService.UpdateProfileAsync(userId, avatarDto);
      return result.Status ? Ok(result) : result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogWarning("Missing X-User-Id header: {Message}", ex.Message);
      return BadRequest(ApiResponse<string>.Fail(ex.Message));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating avatar");
      return StatusCode(500, ApiResponse<string>.Fail("An error occurred while updating avatar."));
    }
  }

  [HttpPost("change-password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        _logger.LogWarning("Invalid change password input: Errors: {Errors}", string.Join(", ", errors));
        return BadRequest(ApiResponse<string>.Fail("Invalid input data."));
      }

      var userId = GetUserId();
      _logger.LogInformation("Changing password for user: {UserId}", userId);
      var result = await _userService.ChangePasswordAsync(userId, changePasswordDto);
      return result.Status ? Ok(result) : BadRequest(result);
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogWarning("Missing X-User-Id header: {Message}", ex.Message);
      return BadRequest(ApiResponse<string>.Fail(ex.Message));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error changing password");
      return StatusCode(500, ApiResponse<string>.Fail("An error occurred while changing password."));
    }
  }

  private string GetUserId()
  {
    var userId = HttpContext.Request.Headers["X-User-Id"].ToString();
    if (string.IsNullOrEmpty(userId))
    {
      throw new InvalidOperationException("User ID is required in X-User-Id header.");
    }
    return userId;
  }
}
