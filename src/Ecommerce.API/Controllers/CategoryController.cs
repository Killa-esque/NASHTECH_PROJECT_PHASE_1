using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
  private readonly ICategoryService _categoryService;
  private readonly IMapper _mapper;

  public CategoryController(ICategoryService categoryService, IMapper mapper)
  {
    _categoryService = categoryService;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    var result = await _categoryService.GetAllCategoriesAsync(pageIndex, pageSize);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    var paged = _mapper.Map<PagedResult<CategoryViewModel>>(result.Data);

    return Ok(ApiResponse<PagedResult<CategoryViewModel>>.Success(paged, result.Message));
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _categoryService.GetCategoryByIdAsync(id);

    if (!result.IsSuccess)
      return NotFound(ApiResponse<string>.Fail(result.Message));

    var viewModel = _mapper.Map<CategoryViewModel>(result.Data);

    return Ok(ApiResponse<CategoryViewModel>.Success(viewModel));
  }
}
