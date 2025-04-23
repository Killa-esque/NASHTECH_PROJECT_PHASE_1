using Ecommerce.Application.Common;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Shared.Common; // ✅ Import ApiResponse
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/admin/products")]
public class AdminProductController : ControllerBase
{
  private readonly IProductService _productService;

  public AdminProductController(IProductService productService)
  {
    _productService = productService;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    var result = await _productService.GetAllProductsAsync(pageIndex, pageSize);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<PagedResult<ProductDto>>.Success(result.Data, result.Message));
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _productService.GetProductByIdAsync(id);

    if (!result.IsSuccess)
      return NotFound(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<ProductDto>.Success(result.Data, result.Message));
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] ProductDto dto)
  {
    var result = await _productService.AddProductAsync(dto);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<string>.Success("Product created successfully."));
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] ProductDto dto)
  {
    dto.Id = id;
    var result = await _productService.UpdateProductAsync(dto);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<string>.Success("Product updated successfully."));
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var result = await _productService.DeleteProductAsync(id);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<string>.Success("Product deleted successfully."));
  }

  [HttpPatch("{id}/set-featured")]
  public async Task<IActionResult> SetFeatured(Guid id, [FromBody] SetFeaturedRequestDto request)
  {
    var result = await _productService.SetProductFeaturedAsync(id, request.IsFeatured);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<string>.Success("Product featured status updated."));
  }
}
