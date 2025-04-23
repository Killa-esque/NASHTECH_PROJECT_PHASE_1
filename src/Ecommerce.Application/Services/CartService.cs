using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Shared.Common;

namespace Ecommerce.Application.Services;

public class CartService : ICartService
{
  private readonly ICartRepository _cartRepository;
  private readonly IProductRepository _productRepository;
  private readonly IMapper _mapper;

  public CartService(ICartRepository cartRepository, IProductRepository productRepository, IMapper mapper)
  {
    _cartRepository = cartRepository;
    _productRepository = productRepository;
    _mapper = mapper;
  }

  public async Task<Result<PagedResult<CartItemDto>>> GetCartItemsAsync(Guid userId, int pageIndex, int pageSize)
  {
    var cartItems = await _cartRepository.GetCartItemsByUserAsync(userId, pageIndex, pageSize);
    var totalCount = await _cartRepository.GetTotalCountByUserAsync(userId);

    var cartItemsDto = _mapper.Map<IEnumerable<CartItemDto>>(cartItems);

    var pagedResult = PagedResult<CartItemDto>.Create(cartItemsDto, totalCount, pageIndex, pageSize);

    return Result.Success(pagedResult, "Cart items retrieved successfully.");
  }

  public async Task<Result> AddToCartAsync(Guid userId, Guid productId, int quantity)
  {
    var product = await _productRepository.GetByIdAsync(productId);
    if (product == null) throw new Exception("Product not found");

    var affectedRows = await _cartRepository.AddOrUpdateCartItemAsync(userId, productId, quantity);

    if (affectedRows != 1)
      return Result.Failure("Failed to add product to cart.");

    return Result.Success("Product added to cart successfully.");
  }

  public async Task<Result> RemoveFromCartAsync(Guid userId, Guid productId)
  {
    var affectedRows = await _cartRepository.RemoveCartItemAsync(userId, productId);

    if (affectedRows != 1)
      return Result.Failure("Failed to remove product from cart.");

    return Result.Success("Product removed from cart successfully.");
  }
}
