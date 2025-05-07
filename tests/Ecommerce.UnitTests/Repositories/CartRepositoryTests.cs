using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Unitest.Repositories
{
  public class CartRepositoryTests
  {
    private readonly AppDbContext _context;
    private readonly ICartRepository _repository;

    public CartRepositoryTests()
    {
      var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;
      _context = new AppDbContext(options);
      _repository = new CartRepository(_context);
    }

    [Fact]
    public async Task GetCartItemsByUserAsync_Success_ReturnsPagedItems()
    {
      // Arrange
      var userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";
      var cartItems = new List<CartItem>
            {
                new CartItem { Id = Guid.NewGuid(), UserId = userId, ProductId = Guid.NewGuid(), Quantity = 2, AddedDate = DateTime.UtcNow },
                new CartItem { Id = Guid.NewGuid(), UserId = userId, ProductId = Guid.NewGuid(), Quantity = 1, AddedDate = DateTime.UtcNow }
            };
      await _context.CartItems.AddRangeAsync(cartItems);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetCartItemsByUserAsync(userId, 1, 10);

      // Assert
      Assert.Equal(2, result.Count());
      Assert.All(result, item => Assert.Equal(userId, item.UserId));
    }

    [Fact]
    public async Task GetCartItemsByUserAsync_EmptyCart_ReturnsEmptyList()
    {
      // Arrange
      var userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";

      // Act
      var result = await _repository.GetCartItemsByUserAsync(userId, 1, 10);

      // Assert
      Assert.Empty(result);
    }

    [Fact]
    public async Task GetTotalCountByUserAsync_Success_ReturnsCorrectCount()
    {
      // Arrange
      var userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";
      var cartItems = new List<CartItem>
            {
                new CartItem { Id = Guid.NewGuid(), UserId = userId, ProductId = Guid.NewGuid(), Quantity = 2, AddedDate = DateTime.UtcNow },
                new CartItem { Id = Guid.NewGuid(), UserId = userId, ProductId = Guid.NewGuid(), Quantity = 1, AddedDate = DateTime.UtcNow }
            };
      await _context.CartItems.AddRangeAsync(cartItems);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetTotalCountByUserAsync(userId);

      // Assert
      Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetTotalCountByUserAsync_EmptyCart_ReturnsZero()
    {
      // Arrange
      var userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";

      // Act
      var result = await _repository.GetTotalCountByUserAsync(userId);

      // Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public async Task AddOrUpdateCartItemAsync_AddNewItem_ReturnsAffectedRows()
    {
      // Arrange
      var userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";
      var productId = Guid.NewGuid();
      var quantity = 2;

      // Act
      var result = await _repository.AddOrUpdateCartItemAsync(userId, productId, quantity);

      // Assert
      Assert.Equal(1, result);
      var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
      Assert.NotNull(cartItem);
      Assert.Equal(quantity, cartItem.Quantity);
    }

    [Fact]
    public async Task AddOrUpdateCartItemAsync_UpdateExistingItem_ReturnsAffectedRows()
    {
      // Arrange
      var userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";
      var productId = Guid.NewGuid();
      var initialCartItem = new CartItem { Id = Guid.NewGuid(), UserId = userId, ProductId = productId, Quantity = 1, AddedDate = DateTime.UtcNow };
      await _context.CartItems.AddAsync(initialCartItem);
      await _context.SaveChangesAsync();
      var newQuantity = 3;

      // Act
      var result = await _repository.AddOrUpdateCartItemAsync(userId, productId, newQuantity, true);

      // Assert
      Assert.Equal(1, result);
      var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
      Assert.NotNull(cartItem);
      Assert.Equal(newQuantity, cartItem.Quantity);
    }

    [Fact]
    public async Task AddOrUpdateCartItemAsync_IncrementQuantity_ReturnsAffectedRows()
    {
      // Arrange
      var userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";
      var productId = Guid.NewGuid();
      var initialCartItem = new CartItem { Id = Guid.NewGuid(), UserId = userId, ProductId = productId, Quantity = 1, AddedDate = DateTime.UtcNow };
      await _context.CartItems.AddAsync(initialCartItem);
      await _context.SaveChangesAsync();
      var additionalQuantity = 2;

      // Act
      var result = await _repository.AddOrUpdateCartItemAsync(userId, productId, additionalQuantity);

      // Assert
      Assert.Equal(1, result);
      var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
      Assert.NotNull(cartItem);
      Assert.Equal(3, cartItem.Quantity);
    }

    [Fact]
    public async Task RemoveCartItemAsync_Success_ReturnsAffectedRows()
    {
      // Arrange
      var userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";
      var productId = Guid.NewGuid();
      var cartItem = new CartItem { Id = Guid.NewGuid(), UserId = userId, ProductId = productId, Quantity = 1, AddedDate = DateTime.UtcNow };
      await _context.CartItems.AddAsync(cartItem);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.RemoveCartItemAsync(userId, productId);

      // Assert
      Assert.Equal(1, result);
      var deletedItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
      Assert.Null(deletedItem);
    }

    [Fact]
    public async Task RemoveCartItemAsync_NotFound_ReturnsZero()
    {
      // Arrange
      var userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";
      var productId = Guid.NewGuid();

      // Act
      var result = await _repository.RemoveCartItemAsync(userId, productId);

      // Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public async Task ClearCartAsync_Success_ReturnsAffectedRows()
    {
      // Arrange
      var userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";
      var cartItems = new List<CartItem>
            {
                new CartItem { Id = Guid.NewGuid(), UserId = userId, ProductId = Guid.NewGuid(), Quantity = 2, AddedDate = DateTime.UtcNow },
                new CartItem { Id = Guid.NewGuid(), UserId = userId, ProductId = Guid.NewGuid(), Quantity = 1, AddedDate = DateTime.UtcNow }
            };
      await _context.CartItems.AddRangeAsync(cartItems);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.ClearCartAsync(userId);

      // Assert
      Assert.Equal(2, result);
      var remainingItems = await _context.CartItems.Where(ci => ci.UserId == userId).ToListAsync();
      Assert.Empty(remainingItems);
    }

    [Fact]
    public async Task ClearCartAsync_EmptyCart_ReturnsZero()
    {
      // Arrange
      var userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";

      // Act
      var result = await _repository.ClearCartAsync(userId);

      // Assert
      Assert.Equal(0, result);
    }
  }
}
