using AutoMapper;
using Ecommerce.API.Requests;
using Ecommerce.Shared.Common;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
  private readonly ICartService _cartService;
  private readonly IMapper _mapper;

  public CartController(ICartService cartService, IMapper mapper)
  {
    _cartService = cartService;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<IActionResult> GetCart([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    var userId = GetUserId(); // lấy từ token hoặc context
    var result = await _cartService.GetCartItemsAsync(userId, pageIndex, pageSize);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    var pagedResult = _mapper.Map<PagedResult<CartItemViewModel>>(result.Data);

    return Ok(ApiResponse<PagedResult<CartItemViewModel>>.Success(pagedResult, result.Message));
  }

  [HttpPost("add")]
  public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
  {
    var userId = GetUserId();
    var result = await _cartService.AddToCartAsync(userId, request.ProductId, request.Quantity);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<string>.Success("Item added to cart."));
  }

  [HttpPost("remove")]
  public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
  {
    var userId = GetUserId();
    var result = await _cartService.RemoveFromCartAsync(userId, request.ProductId);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<string>.Success("Item removed from cart."));
  }

  private string GetUserId()
  {
    return "b831506c-d805-4b6b-8682-74892b7f86e7";
    // return Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
  }
}
