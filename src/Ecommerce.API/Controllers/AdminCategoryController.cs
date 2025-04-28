using AutoMapper;
using Ecommerce.Shared.Common;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Shared.Common; // ✅ Import ApiResponse
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/admin/categories")]
public class AdminCategoryController : ControllerBase
{
  private readonly ICategoryService _categoryService;

  public AdminCategoryController(ICategoryService categoryService)
  {
    _categoryService = categoryService;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    var result = await _categoryService.GetAllCategoriesAsync(pageIndex, pageSize);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<object>.Success(result.Data, result.Message)); // result.Data là PagedResult<CategoryDto>
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _categoryService.GetCategoryByIdAsync(id);

    if (!result.IsSuccess)
      return NotFound(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<CategoryDto>.Success(result.Data, result.Message));
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CategoryDto dto)
  {
    var result = await _categoryService.AddCategoryAsync(dto);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<string>.Success(result.Message));
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDto dto)
  {
    dto.Id = id;
    var result = await _categoryService.UpdateCategoryAsync(dto);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<string>.Success(result.Message));
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var result = await _categoryService.DeleteCategoryAsync(id);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<string>.Success(result.Message));
  }
}
