using AutoMapper;
using Ecommerce.Shared.DTOs;
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

  public async Task<Result<PagedResult<CartItemDto>>> GetCartItemsAsync(string userId, int pageIndex, int pageSize)
  {
    var cartItems = await _cartRepository.GetCartItemsByUserAsync(userId, pageIndex, pageSize);
    var totalCount = await _cartRepository.GetTotalCountByUserAsync(userId);

    var cartItemsDto = new List<CartItemDto>();
    foreach (var cartItem in cartItems)
    {
      var cartItemDto = _mapper.Map<CartItemDto>(cartItem);

      // Fetch the product to get the name, price, and image
      var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
      if (product != null)
      {
        cartItemDto.ProductName = product.Name;
        cartItemDto.Price = product.Price;

        // Fetch the product image
        var images = await _productRepository.GetProductImagesAsync(cartItem.ProductId);
        cartItemDto.ImageUrl = images?.FirstOrDefault()?.ImageUrl ?? "https://example.com/images/default-product.jpg"; // Fallback image
      }
      else
      {
        cartItemDto.ProductName = "Unknown Product";
        cartItemDto.Price = 0;
        cartItemDto.ImageUrl = "https://example.com/images/default-product.jpg";
      }

      cartItemsDto.Add(cartItemDto);
    }

    var pagedResult = PagedResult<CartItemDto>.Create(cartItemsDto, totalCount, pageIndex, pageSize);
    return Result.Success(pagedResult, "Cart items retrieved successfully.");
  }

  public async Task<Result> AddToCartAsync(string userId, Guid productId, int quantity)
  {
    var product = await _productRepository.GetByIdAsync(productId);
    if (product == null) return Result.Failure("Product not found");

    var affectedRows = await _cartRepository.AddOrUpdateCartItemAsync(userId, productId, quantity);

    if (affectedRows != 1)
      return Result.Failure("Failed to add product to cart.");

    return Result.Success("Product added to cart successfully.");
  }

  public async Task<Result> RemoveFromCartAsync(string userId, Guid productId)
  {
    var affectedRows = await _cartRepository.RemoveCartItemAsync(userId, productId);

    if (affectedRows != 1)
      return Result.Failure("Failed to remove product from cart.");

    return Result.Success("Product removed from cart successfully.");
  }

  public async Task<Result> UpdateCartItemAsync(string userId, Guid productId, int quantity)
  {
    if (quantity < 1) return Result.Failure("Quantity must be at least 1");

    var product = await _productRepository.GetByIdAsync(productId);
    if (product == null) return Result.Failure("Product not found");

    var affectedRows = await _cartRepository.AddOrUpdateCartItemAsync(userId, productId, quantity, true);

    if (affectedRows != 1)
      return Result.Failure("Failed to update cart item.");

    return Result.Success("Cart item updated successfully.");
  }
}
