using AutoMapper;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;
[ApiController]
[Route("api/ratings")]
// [Authorize] // User phải đăng nhập
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
  public async Task<IActionResult> GetRatingsForProduct(Guid productId)
  {
    var result = await _ratingService.GetRatingsByProductAsync(productId);

    if (result.IsSuccess)
      return Ok(ApiResponse<PagedResult<RatingDto>>.Success(result.Data, result.Message));

    return NotFound(ApiResponse<string>.Fail(result.Error));
  }

  private string GetUserId()
  {
    return "b831506c-d805-4b6b-8682-74892b7f86e7";
  }
}
