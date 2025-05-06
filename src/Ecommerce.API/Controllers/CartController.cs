using Ecommerce.Shared.Requests;
using Ecommerce.Shared.Common;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/cart")]
// [Authorize] // Ensure only logged-in users can call
public class CartController : ControllerBase
{
  private readonly ICartService _cartService;

  public CartController(ICartService cartService)
  {
    _cartService = cartService;
  }

  [HttpGet]
  public async Task<IActionResult> GetCart([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    var userId = GetUserId();
    var result = await _cartService.GetCartItemsAsync(userId, pageIndex, pageSize);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<PagedResult<CartItemDto>>.Success(result.Data, result.Message));
  }

  [HttpPost("add")]
  public async Task<IActionResult> AddToCart([FromBody] CartRequest request)
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

  [HttpPut("update")]
  public async Task<IActionResult> UpdateCartItem([FromBody] CartRequest request)
  {
    var userId = GetUserId();
    var result = await _cartService.UpdateCartItemAsync(userId, request.ProductId, request.Quantity);

    if (!result.IsSuccess)
      return BadRequest(ApiResponse<string>.Fail(result.Message));

    return Ok(ApiResponse<string>.Success("Cart item updated successfully."));
  }

  private string GetUserId()
  {
    // Later, use: return User.FindFirst("sub")?.Value
    return "a8a9b907-81a6-41c9-90ff-eb2158c52044";
  }
}
