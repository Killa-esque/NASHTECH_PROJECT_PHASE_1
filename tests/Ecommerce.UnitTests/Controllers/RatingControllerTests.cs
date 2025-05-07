using AutoMapper;
using Ecommerce.API.Controllers;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ecommerce.Unitest.Controllers
{
  public class RatingControllerTests
  {
    private readonly Mock<IRatingService> _mockRatingService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly RatingController _controller;

    public RatingControllerTests()
    {
      _mockRatingService = new Mock<IRatingService>();
      _mockMapper = new Mock<IMapper>();
      _controller = new RatingController(_mockRatingService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateRating_Success_ReturnsOk()
    {
      // Arrange
      var userId = "a2ad953e-1482-4cf2-9b94-41c88aeb90e3";
      var request = new CreateRatingDto
      {
        ProductId = Guid.NewGuid(),
        RatingValue = 5,
        Comment = "Great product!"
      };
      var ratingId = Guid.NewGuid();
      var result = Result.Success(ratingId, "Rating created successfully.");
      _mockRatingService.Setup(s => s.CreateRatingAsync(userId, request))
                       .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.CreateRating(request);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<Guid>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(ratingId, apiResponse.Data);
      Assert.Equal("Rating created successfully.", apiResponse.Message);
    }

    [Fact]
    public async Task CreateRating_Failure_ReturnsBadRequest()
    {
      // Arrange
      var userId = "a2ad953e-1482-4cf2-9b94-41c88aeb90e3";
      var request = new CreateRatingDto
      {
        ProductId = Guid.NewGuid(),
        RatingValue = 5,
        Comment = "Great product!"
      };
      var result = Result.Failure<Guid>("Product not found.");
      _mockRatingService.Setup(s => s.CreateRatingAsync(userId, request))
                       .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.CreateRating(request);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Equal("Product not found.", apiResponse.Message);
    }

    [Fact]
    public async Task GetRatingsForProduct_Success_ReturnsOk()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var pageIndex = 1;
      var pageSize = 10;
      var ratings = new List<RatingDto>
            {
                new RatingDto
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    UserId = "a2ad953e-1482-4cf2-9b94-41c88aeb90e3",
                    UserName = "TestUser",
                    RatingValue = 5,
                    Comment = "Great product!",
                    CreatedDate = DateTime.UtcNow
                }
            };
      var pagedResult = PagedResult<RatingDto>.Create(ratings, 1, pageIndex, pageSize);
      var result = Result.Success(pagedResult, "Ratings retrieved successfully.");
      _mockRatingService.Setup(s => s.GetRatingsByProductAsync(productId, pageIndex, pageSize))
                       .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.GetRatingsForProduct(productId, pageIndex, pageSize);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<PagedResult<RatingDto>>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(pagedResult, apiResponse.Data);
      Assert.Equal("Ratings retrieved successfully.", apiResponse.Message);
    }

    [Fact]
    public async Task GetRatingsForProduct_Failure_ReturnsNotFound()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var pageIndex = 1;
      var pageSize = 10;
      var result = Result.Failure<PagedResult<RatingDto>>("Ratings not found.");
      _mockRatingService.Setup(s => s.GetRatingsByProductAsync(productId, pageIndex, pageSize))
                       .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.GetRatingsForProduct(productId, pageIndex, pageSize);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Equal("Ratings not found.", apiResponse.Message);
    }
  }
}
