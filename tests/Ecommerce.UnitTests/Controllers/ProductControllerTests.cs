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
  public class ProductControllerTests
  {
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
      _mockProductService = new Mock<IProductService>();
      _mockMapper = new Mock<IMapper>();
      _controller = new ProductController(_mockProductService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllProducts_Success_ReturnsOkWithPagedResult()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var pagedResult = new PagedResult<ProductDto> { Items = new List<ProductDto>(), TotalCount = 0 };
      var result = Result.Success(pagedResult, "Success");
      _mockProductService.Setup(s => s.GetAllProductsAsync(pageIndex, pageSize))
                        .ReturnsAsync(result);
      _mockMapper.Setup(m => m.Map<PagedResult<ProductDto>>(pagedResult))
                 .Returns(pagedResult);

      // Act
      var actionResult = await _controller.GetAllProducts(pageIndex, pageSize);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<PagedResult<ProductDto>>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(pagedResult, apiResponse.Data);
      Assert.Equal("Success", apiResponse.Message);
    }

    [Fact]
    public async Task GetAllProducts_Failure_ReturnsBadRequest()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var result = Result.Failure<PagedResult<ProductDto>>(null); // Message is null
      _mockProductService.Setup(s => s.GetAllProductsAsync(pageIndex, pageSize))
                        .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.GetAllProducts(pageIndex, pageSize);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Null(apiResponse.Message); // Check for null instead of ""
    }

    [Fact]
    public async Task GetFeaturedProducts_Success_ReturnsOkWithPagedResult()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var pagedResult = new PagedResult<ProductDto> { Items = new List<ProductDto>(), TotalCount = 0 };
      var result = Result.Success(pagedResult, "Success");
      _mockProductService.Setup(s => s.GetFeaturedProductsAsync(pageIndex, pageSize))
                        .ReturnsAsync(result);
      _mockMapper.Setup(m => m.Map<PagedResult<ProductDto>>(pagedResult))
                 .Returns(pagedResult);

      // Act
      var actionResult = await _controller.GetFeaturedProducts(pageIndex, pageSize);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<PagedResult<ProductDto>>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(pagedResult, apiResponse.Data);
    }

    [Fact]
    public async Task GetFeaturedProducts_Failure_ReturnsBadRequest()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var result = Result.Failure<PagedResult<ProductDto>>(null); // Message is null
      _mockProductService.Setup(s => s.GetFeaturedProductsAsync(pageIndex, pageSize))
                        .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.GetFeaturedProducts(pageIndex, pageSize);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Null(apiResponse.Message); // Check for null instead of ""
    }

    [Fact]
    public async Task GetProductsByCategory_Success_ReturnsOkWithPagedResult()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var pageIndex = 1;
      var pageSize = 10;
      var pagedResult = new PagedResult<ProductDto> { Items = new List<ProductDto>(), TotalCount = 0 };
      var result = Result.Success(pagedResult, "Success");
      _mockProductService.Setup(s => s.GetProductsByCategoryAsync(categoryId, pageIndex, pageSize))
                        .ReturnsAsync(result);
      _mockMapper.Setup(m => m.Map<PagedResult<ProductDto>>(pagedResult))
                 .Returns(pagedResult);

      // Act
      var actionResult = await _controller.GetProductsByCategory(categoryId, pageIndex, pageSize);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<PagedResult<ProductDto>>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(pagedResult, apiResponse.Data);
    }

    [Fact]
    public async Task GetProductById_Success_ReturnsOkWithProductDto()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var productDto = new ProductDto { Id = productId, Name = "Cake" };
      var result = Result.Success(productDto, "Success");
      _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
                        .ReturnsAsync(result);
      _mockMapper.Setup(m => m.Map<ProductDto>(productDto))
                 .Returns(productDto);

      // Act
      var actionResult = await _controller.GetProductById(productId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<ProductDto>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(productDto, apiResponse.Data);
    }

    [Fact]
    public async Task GetProductById_Failure_ReturnsNotFound()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var result = Result.Failure<ProductDto>(null); // Message is null
      _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
                        .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.GetProductById(productId);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Null(apiResponse.Message); // Check for null instead of ""
    }
  }
}
