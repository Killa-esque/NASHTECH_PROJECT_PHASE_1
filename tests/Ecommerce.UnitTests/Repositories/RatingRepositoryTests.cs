using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Unitest.Repositories
{
  public class RatingRepositoryTests
  {
    private readonly AppDbContext _context;
    private readonly RatingRepository _repository;

    public RatingRepositoryTests()
    {
      var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;
      _context = new AppDbContext(options);
      _repository = new RatingRepository(_context);
    }

    [Fact]
    public async Task GetRatingsByProductIdAsync_ValidProductId_ReturnsRatings()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var userId = "a2ad953e-1482-4cf2-9b94-41c88aeb90e3";
      var ratings = new List<Rating>
            {
                new Rating
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    UserId = userId,
                    RatingValue = 5,
                    Comment = "Great product!",
                    CreatedDate = DateTime.UtcNow
                }
            };
      var user = new ApplicationUser
      {
        Id = userId,
        UserName = "TestUser",
        NormalizedUserName = "TESTUSER",
        Email = "testuser@example.com",
        NormalizedEmail = "TESTUSER@EXAMPLE.COM",
        FullName = "Test User",
        AllergyNotes = "",
        AvatarUrl = "",
        DefaultAddress = "",
        Gender = "Unknown"
      };
      await _context.Users.AddAsync(user);
      await _context.Ratings.AddRangeAsync(ratings);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetRatingsByProductIdAsync(productId, 1, 10);

      // Assert
      Assert.Single(result);
      Assert.Equal(ratings[0].Id, result.First().Id);
      Assert.Equal(ratings[0].ProductId, result.First().ProductId);
    }

    [Fact]
    public async Task GetRatingsByProductIdAsync_NoRatings_ReturnsEmpty()
    {
      // Arrange
      var productId = Guid.NewGuid();

      // Act
      var result = await _repository.GetRatingsByProductIdAsync(productId, 1, 10);

      // Assert
      Assert.Empty(result);
    }

    [Fact]
    public async Task AddAsync_Success_AddsRating()
    {
      // Arrange
      var rating = new Rating
      {
        Id = Guid.NewGuid(),
        ProductId = Guid.NewGuid(),
        UserId = "a2ad953e-1482-4cf2-9b94-41c88aeb90e3",
        RatingValue = 5,
        Comment = "Great product!",
        CreatedDate = DateTime.UtcNow
      };

      // Act
      await _repository.AddAsync(rating);
      await _repository.SaveChangesAsync();

      // Assert
      var savedRating = await _context.Ratings.FindAsync(rating.Id);
      Assert.NotNull(savedRating);
      Assert.Equal(rating.Id, savedRating.Id);
      Assert.Equal(rating.ProductId, savedRating.ProductId);
    }

    [Fact]
    public async Task GetUserNameByIdAsync_ValidUserId_ReturnsUserName()
    {
      // Arrange
      var userId = "a2ad953e-1482-4cf2-9b94-41c88aeb90e3";
      var user = new ApplicationUser
      {
        Id = userId,
        UserName = "TestUser",
        NormalizedUserName = "TESTUSER",
        Email = "testuser@example.com",
        NormalizedEmail = "TESTUSER@EXAMPLE.COM",
        FullName = "Test User",
        AllergyNotes = "",
        AvatarUrl = "",
        DefaultAddress = "",
        Gender = "Unknown"
      };
      await _context.Users.AddAsync(user);
      await _context.SaveChangesAsync();

      // Act
      var userName = await _repository.GetUserNameByIdAsync(userId);

      // Assert
      Assert.Equal("TestUser", userName);
    }

    [Fact]
    public async Task GetUserNameByIdAsync_InvalidUserId_ReturnsUnknown()
    {
      // Arrange
      var userId = "invalid-user-id";

      // Act
      var userName = await _repository.GetUserNameByIdAsync(userId);

      // Assert
      Assert.Equal("Unknown User", userName);
    }

    [Fact]
    public async Task CountRatingsByProductIdAsync_ValidProductId_ReturnsCount()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var ratings = new List<Rating>
            {
                new Rating
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    UserId = "user1",
                    RatingValue = 5,
                    Comment = "Good product"
                },
                new Rating
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    UserId = "user2",
                    RatingValue = 4,
                    Comment = "Nice product"
                }
            };
      await _context.Ratings.AddRangeAsync(ratings);
      await _context.SaveChangesAsync();

      // Act
      var count = await _repository.CountRatingsByProductIdAsync(productId);

      // Assert
      Assert.Equal(2, count);
    }

    [Fact]
    public async Task CountRatingsByProductIdAsync_NoRatings_ReturnsZero()
    {
      // Arrange
      var productId = Guid.NewGuid();

      // Act
      var count = await _repository.CountRatingsByProductIdAsync(productId);

      // Assert
      Assert.Equal(0, count);
    }
  }
}
