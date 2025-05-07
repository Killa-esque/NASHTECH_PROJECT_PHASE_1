using AutoMapper;
using Ecommerce.Shared.DTOs;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/ratings")]
// [Authorize] // User must be logged in
public class RatingController : ControllerBase
{
  private readonly IRatingService _ratingService;
  private readonly IMapper _mapper;

  public RatingController(IRatingService ratingService, IMapper mapper)
  {
    _ratingService = ratingService;
    _mapper = mapper;
  }

  [HttpPost]
  public async Task<IActionResult> CreateRating([FromBody] CreateRatingDto request)
  {
    var userId = GetUserId();
    var result = await _ratingService.CreateRatingAsync(userId, request);

    if (result.IsSuccess)
      return Ok(ApiResponse<Guid>.Success(result.Data, result.Message));

    return BadRequest(ApiResponse<string>.Fail(result.Error));
  }

  [HttpGet("product/{productId}")]
  public async Task<IActionResult> GetRatingsForProduct(Guid productId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    var result = await _ratingService.GetRatingsByProductAsync(productId, pageIndex, pageSize);

    if (result.IsSuccess)
      return Ok(ApiResponse<PagedResult<RatingDto>>.Success(result.Data, result.Message));

    return NotFound(ApiResponse<string>.Fail(result.Error));
  }

  private string GetUserId()
  {
    return "4b879495-4cc3-42e0-a5da-1bafa9aa8e05";
  }
}
