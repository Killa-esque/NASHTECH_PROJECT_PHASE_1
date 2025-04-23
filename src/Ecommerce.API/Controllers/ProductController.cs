using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
  private readonly IProductService _productService;
  private readonly IMapper _mapper;

  public ProductController(IProductService productService, IMapper mapper)
  {
    _productService = productService;
    _mapper = mapper;
  }

  [HttpGet("featured")]
  public async Task<IActionResult> GetFeaturedProducts([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    var result = await _productService.GetFeaturedProductsAsync(pageIndex, pageSize);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));


    var paged = _mapper.Map<PagedResult<ProductViewModel>>(result.Data);

    return Ok(ApiResponse<PagedResult<ProductViewModel>>.Success(paged, result.Message));
  }

  [HttpGet("category/{categoryId}")]
  public async Task<IActionResult> GetProductsByCategory(Guid categoryId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    var result = await _productService.GetProductsByCategoryAsync(categoryId, pageIndex, pageSize);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    var paged = _mapper.Map<PagedResult<ProductViewModel>>(result.Data);

    return Ok(ApiResponse<PagedResult<ProductViewModel>>.Success(paged, result.Message));
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetProductById(Guid id)
  {
    var result = await _productService.GetProductByIdAsync(id);

    if (!result.IsSuccess)
      return NotFound(ApiResponse<string>.Fail(result.Message));

    var viewModel = _mapper.Map<ProductViewModel>(result.Data);
    return Ok(ApiResponse<ProductViewModel>.Success(viewModel, result.Message));
  }
}
