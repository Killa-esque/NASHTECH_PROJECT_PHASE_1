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
  public class AdminCategoryControllerTests
  {
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly AdminCategoryController _controller;

    public AdminCategoryControllerTests()
    {
      _mockCategoryService = new Mock<ICategoryService>();
      _controller = new AdminCategoryController(_mockCategoryService.Object);
    }

    [Fact]
    public async Task GetAll_Success_ReturnsOkWithPagedResult()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var pagedResult = new PagedResult<CategoryDto>
      {
        Items = new List<CategoryDto> { new CategoryDto { Id = Guid.NewGuid(), Name = "Category1" } },
        TotalCount = 1,
        PageIndex = pageIndex,
        PageSize = pageSize
      };
      var result = Result.Success(pagedResult, "Categories retrieved successfully.");
      _mockCategoryService.Setup(s => s.GetAllCategoriesAsync(pageIndex, pageSize))
                         .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.GetAll(pageIndex, pageSize);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(pagedResult, apiResponse.Data);
      Assert.Equal("Categories retrieved successfully.", apiResponse.Message);
    }

    [Fact]
    public async Task GetAll_Failure_ReturnsBadRequest()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var result = Result.Failure<PagedResult<CategoryDto>>(null); // Accept null Message
      _mockCategoryService.Setup(s => s.GetAllCategoriesAsync(pageIndex, pageSize))
                         .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.GetAll(pageIndex, pageSize);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Null(apiResponse.Message); // Expect null
    }

    [Fact]
    public async Task GetById_Success_ReturnsOkWithCategoryDto()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var categoryDto = new CategoryDto { Id = categoryId, Name = "Category1" };
      var result = Result.Success(categoryDto, "Category retrieved successfully.");
      _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(categoryId))
                         .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.GetById(categoryId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<CategoryDto>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(categoryDto, apiResponse.Data);
      Assert.Equal("Category retrieved successfully.", apiResponse.Message);
    }

    [Fact]
    public async Task GetById_Failure_ReturnsNotFound()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var result = Result.Failure<CategoryDto>(null); // Accept null Message
      _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(categoryId))
                         .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.GetById(categoryId);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Null(apiResponse.Message); // Expect null
    }

    [Fact]
    public async Task Create_Success_ReturnsOk()
    {
      // Arrange
      var categoryDto = new CategoryDto { Name = "Category1", Description = "Description" };
      var result = Result.Success("Success"); // Match actual behavior
      _mockCategoryService.Setup(s => s.AddCategoryAsync(categoryDto))
                         .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.Create(categoryDto);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Success", apiResponse.Message);
    }

    [Fact]
    public async Task Create_Failure_ReturnsBadRequest()
    {
      // Arrange
      var categoryDto = new CategoryDto { Name = "Category1", Description = "Description" };
      var result = Result.Failure(null); // Accept null Message
      _mockCategoryService.Setup(s => s.AddCategoryAsync(categoryDto))
                         .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.Create(categoryDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Null(apiResponse.Message); // Expect null
    }

    [Fact]
    public async Task Update_Success_ReturnsOk()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var categoryDto = new CategoryDto { Id = categoryId, Name = "Updated Category" };
      var result = Result.Success("Success"); // Match actual behavior
      _mockCategoryService.Setup(s => s.UpdateCategoryAsync(categoryDto))
                         .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.Update(categoryId, categoryDto);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Success", apiResponse.Message);
    }

    [Fact]
    public async Task Update_Failure_ReturnsBadRequest()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var categoryDto = new CategoryDto { Id = categoryId, Name = "Updated Category" };
      var result = Result.Failure(null); // Accept null Message
      _mockCategoryService.Setup(s => s.UpdateCategoryAsync(categoryDto))
                         .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.Update(categoryId, categoryDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Null(apiResponse.Message); // Expect null
    }

    [Fact]
    public async Task Delete_Success_ReturnsOk()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var result = Result.Success("Success"); // Match actual behavior
      _mockCategoryService.Setup(s => s.DeleteCategoryAsync(categoryId))
                         .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.Delete(categoryId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Success", apiResponse.Message);
    }

    [Fact]
    public async Task Delete_Failure_ReturnsBadRequest()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var result = Result.Failure(null); // Accept null Message
      _mockCategoryService.Setup(s => s.DeleteCategoryAsync(categoryId))
                         .ReturnsAsync(result);

      // Act
      var actionResult = await _controller.Delete(categoryId);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Null(apiResponse.Message); // Expect null
    }
  }
}
