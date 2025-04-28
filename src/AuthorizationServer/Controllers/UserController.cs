using AutoMapper;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Shared.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Ecommerce.Shared.DTOs;

namespace AuthorizationServer.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IMapper _mapper;

  public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper)
  {
    _userManager = userManager;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    if (pageIndex < 1 || pageSize <= 0)
      return BadRequest(ApiResponse<string>.Fail("Invalid paging parameters."));

    var users = await _userManager.Users
        .AsNoTracking()
        .OrderBy(u => u.UserName)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(users);
    var totalCount = await _userManager.Users.CountAsync();

    var pagedResult = PagedResult<CustomerDto>.Create(customerDtos, totalCount, pageIndex, pageSize);

    return Ok(ApiResponse<PagedResult<CustomerDto>>.Success(pagedResult, "Users retrieved successfully."));
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(string id)
  {
    if (string.IsNullOrEmpty(id))
      return BadRequest(ApiResponse<string>.Fail("User ID is required."));

    var user = await _userManager.FindByIdAsync(id);
    if (user == null)
      return NotFound(ApiResponse<string>.Fail("User not found."));

    var customerDto = _mapper.Map<CustomerDto>(user);
    return Ok(ApiResponse<CustomerDto>.Success(customerDto, "User retrieved successfully."));
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateCustomerDto updateDto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ApiResponse<string>.Fail("Invalid input data."));

    if (string.IsNullOrEmpty(id))
      return BadRequest(ApiResponse<string>.Fail("User ID is required."));

    var user = await _userManager.FindByIdAsync(id);
    if (user == null)
      return NotFound(ApiResponse<string>.Fail("User not found."));

    // Xử lý DateOfBirth
    if (!string.IsNullOrEmpty(updateDto.DateOfBirth))
    {
      try
      {
        user.DateOfBirth = DateTime.ParseExact(updateDto.DateOfBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture).Date;
      }
      catch (FormatException)
      {
        return BadRequest(ApiResponse<string>.Fail("DateOfBirth must be in format dd-MM-yyyy."));
      }
    }
    else
    {
      user.DateOfBirth = null;
    }

    // Ánh xạ các thuộc tính khác
    _mapper.Map(updateDto, user);

    var result = await _userManager.UpdateAsync(user);
    if (!result.Succeeded)
    {
      var errors = string.Join(", ", result.Errors.Select(e => e.Description));
      return BadRequest(ApiResponse<string>.Fail($"Failed to update user: {errors}"));
    }

    var updatedCustomerDto = _mapper.Map<CustomerDto>(user);
    return Ok(ApiResponse<CustomerDto>.Success(updatedCustomerDto, "User updated successfully."));
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUser(string id)
  {
    if (string.IsNullOrEmpty(id))
      return BadRequest(ApiResponse<string>.Fail("User ID is required."));

    var user = await _userManager.FindByIdAsync(id);
    if (user == null)
      return NotFound(ApiResponse<string>.Fail("User not found."));

    var result = await _userManager.DeleteAsync(user);
    if (!result.Succeeded)
    {
      var errors = string.Join(", ", result.Errors.Select(e => e.Description));
      return BadRequest(ApiResponse<string>.Fail($"Failed to delete user: {errors}"));
    }

    return Ok(ApiResponse<bool>.Success(true, "User deleted successfully."));
  }
}
