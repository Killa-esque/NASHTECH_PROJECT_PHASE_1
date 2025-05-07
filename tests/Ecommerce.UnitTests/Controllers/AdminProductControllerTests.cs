using Ecommerce.API.Controllers;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ecommerce.Unitest.Controllers
{
  public class AdminProductControllerTests
  {
    private readonly Mock<IProductService> _mockProductService;
    private readonly AdminProductController _controller;

    public AdminProductControllerTests()
    {
      _mockProductService = new Mock<IProductService>();
      _controller = new AdminProductController(_mockProductService.Object);
    }

    [Fact]
    public async Task GetAll_Success_ReturnsOkWithPagedResult()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var pagedResult = new PagedResult<ProductDto> { Items = new List<ProductDto>(), TotalCount = 0 };
      var result = Result.Success(pagedResult, "Success");
      _mockProductService.Setup(s => s.GetAllProductsAsync(pageIndex, pageSize))
                        .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.GetAll(pageIndex, pageSize);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<PagedResult<ProductDto>>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(pagedResult, apiResponse.Data);
    }

    [Fact]
    public async Task GetById_Success_ReturnsOkWithProductDto()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var productDto = new ProductDto { Id = productId, Name = "Cake" };
      var result = Result.Success(productDto, "Success");
      _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
                        .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.GetById(productId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<ProductDto>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(productDto, apiResponse.Data);
    }

    [Fact]
    public async Task Create_Success_ReturnsOk()
    {
      // Arrange
      var productDto = new ProductDto { Name = "Cake" };
      var result = Result.Success(Guid.NewGuid(), "Product created");
      _mockProductService.Setup(s => s.AddProductAsync(productDto))
                        .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.Create(productDto);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Product created successfully.", apiResponse.Data);
    }

    [Fact]
    public async Task Update_Success_ReturnsOk()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var productDto = new ProductDto { Id = productId, Name = "Cake" };
      var result = Result.Success("Product updated");
      _mockProductService.Setup(s => s.UpdateProductAsync(productDto))
                        .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.Update(productId, productDto);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Product updated successfully.", apiResponse.Data);
    }

    [Fact]
    public async Task Delete_Success_ReturnsOk()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var result = Result.Success("Product deleted");
      _mockProductService.Setup(s => s.DeleteProductAsync(productId))
                        .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.Delete(productId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Product deleted successfully.", apiResponse.Data);
    }

    [Fact]
    public async Task SetFeatured_Success_ReturnsOk()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var request = new SetFeaturedRequestDto { IsFeatured = true };
      var result = Result.Success("Product updated");
      _mockProductService.Setup(s => s.SetProductFeaturedAsync(productId, request.IsFeatured))
                        .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.SetFeatured(productId, request);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Product featured status updated.", apiResponse.Data);
    }

    [Fact]
    public async Task UploadImages_Success_ReturnsOk()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var formFile = new Mock<IFormFile>();
      formFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
      formFile.Setup(f => f.FileName).Returns("test.jpg");
      formFile.Setup(f => f.ContentType).Returns("image/jpeg");
      var files = new List<IFormFile> { formFile.Object };
      var result = Result.Success("Images uploaded");
      _mockProductService.Setup(s => s.AddProductImagesAsync(productId, It.IsAny<List<(Stream, string, string)>>()))
                        .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.UploadImages(productId, files);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Images uploaded.", apiResponse.Data);
    }

    [Fact]
    public async Task DeleteImage_Success_ReturnsOk()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var imageUrl = "http://example.com/image.jpg";
      var result = Result.Success("Image deleted");
      _mockProductService.Setup(s => s.DeleteProductImageAsync(productId, imageUrl))
                        .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.DeleteImage(productId, imageUrl);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Image deleted.", apiResponse.Data);
    }
  }
}
