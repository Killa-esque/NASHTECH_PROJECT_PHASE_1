using AutoMapper;
using Ecommerce.API.Controllers;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ecommerce.Unitest.Controllers
{
  public class CategoryControllerTests
  {
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
      _mockCategoryService = new Mock<ICategoryService>();
      _mockMapper = new Mock<IMapper>();
      _controller = new CategoryController(_mockCategoryService.Object, _mockMapper.Object);
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
      var viewModelResult = new PagedResult<CategoryViewModel>
      {
        Items = new List<CategoryViewModel> { new CategoryViewModel { Id = pagedResult.Items.First().Id, Name = "Category1" } },
        TotalCount = 1,
        PageIndex = pageIndex,
        PageSize = pageSize
      };
      var result = Result.Success(pagedResult, "Categories retrieved successfully.");
      _mockCategoryService.Setup(s => s.GetAllCategoriesAsync(pageIndex, pageSize))
                         .ReturnsAsync(result);
      _mockMapper.Setup(m => m.Map<PagedResult<CategoryViewModel>>(pagedResult))
                 .Returns(viewModelResult);

      // Act
      var actionResult = await _controller.GetAll(pageIndex, pageSize);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<PagedResult<CategoryViewModel>>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(viewModelResult, apiResponse.Data);
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
    public async Task GetById_Success_ReturnsOkWithCategoryViewModel()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var categoryDto = new CategoryDto { Id = categoryId, Name = "Category1" };
      var viewModel = new CategoryViewModel { Id = categoryId, Name = "Category1" };
      var result = Result.Success(categoryDto, "Success"); // Match actual service behavior
      _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(categoryId))
                         .ReturnsAsync(result);
      _mockMapper.Setup(m => m.Map<CategoryViewModel>(categoryDto))
                 .Returns(viewModel);

      // Act
      var actionResult = await _controller.GetById(categoryId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<CategoryViewModel>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(viewModel, apiResponse.Data);
      Assert.Equal("Success", apiResponse.Message);
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
  }
}
