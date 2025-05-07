using AutoMapper;
using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Application.Services;
using Ecommerce.Domain.Entities;
using Ecommerce.Shared.DTOs;
using Moq;

namespace Ecommerce.Unitest.Services
{
  public class CategoryServiceTests
  {
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
      _mockCategoryRepository = new Mock<ICategoryRepository>();
      _mockMapper = new Mock<IMapper>();
      _service = new CategoryService(_mockCategoryRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_Success_ReturnsPagedResult()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Category1", Description = "Desc1" }
            };
      var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto { Id = categories[0].Id, Name = "Category1", Description = "Desc1" }
            };
      _mockCategoryRepository.Setup(r => r.GetAllAsync(pageIndex, pageSize)).ReturnsAsync(categories);
      _mockCategoryRepository.Setup(r => r.CountAsync()).ReturnsAsync(1);
      _mockMapper.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories)).Returns(categoryDtos);

      // Act
      var result = await _service.GetAllCategoriesAsync(pageIndex, pageSize);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Categories retrieved successfully.", result.Message);
      Assert.Equal(1, result.Data.TotalCount);
      Assert.Single(result.Data.Items);
      Assert.Equal(categoryDtos[0].Id, result.Data.Items.First().Id);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_Success_ReturnsCategoryDto()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var category = new Category { Id = categoryId, Name = "Category1", Description = "Desc1" };
      var categoryDto = new CategoryDto { Id = categoryId, Name = "Category1", Description = "Desc1" };
      _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
      _mockMapper.Setup(m => m.Map<CategoryDto>(category)).Returns(categoryDto);

      // Act
      var result = await _service.GetCategoryByIdAsync(categoryId);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Category retrieved successfully.", result.Message);
      Assert.Equal(categoryDto, result.Data);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_NotFound_ReturnsFailure()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category)null);

      // Act
      var result = await _service.GetCategoryByIdAsync(categoryId);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Null(result.Message); // Accept null Message
      Assert.Null(result.Data);
    }

    [Fact]
    public async Task AddCategoryAsync_Success_ReturnsSuccess()
    {
      // Arrange
      var categoryDto = new CategoryDto { Name = "Category1", Description = "Desc1" };
      var category = new Category { Id = Guid.NewGuid(), Name = "Category1", Description = "Desc1" };
      _mockMapper.Setup(m => m.Map<Category>(categoryDto)).Returns(category);
      _mockCategoryRepository.Setup(r => r.ExistsAsync(category.Name)).ReturnsAsync(false);
      _mockCategoryRepository.Setup(r => r.AddAsync(category)).ReturnsAsync(1);

      // Act
      var result = await _service.AddCategoryAsync(categoryDto);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Category added successfully.", result.Message);
    }

    [Fact]
    public async Task AddCategoryAsync_NullDto_ReturnsFailure()
    {
      // Arrange
      CategoryDto categoryDto = null;

      // Act
      var result = await _service.AddCategoryAsync(categoryDto);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Null(result.Message); // Accept null Message
    }

    [Fact]
    public async Task AddCategoryAsync_Exists_ReturnsFailure()
    {
      // Arrange
      var categoryDto = new CategoryDto { Name = "Category1", Description = "Desc1" };
      var category = new Category { Id = Guid.NewGuid(), Name = "Category1", Description = "Desc1" };
      _mockMapper.Setup(m => m.Map<Category>(categoryDto)).Returns(category);
      _mockCategoryRepository.Setup(r => r.ExistsAsync(category.Name)).ReturnsAsync(true);

      // Act
      var result = await _service.AddCategoryAsync(categoryDto);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Null(result.Message); // Accept null Message
    }

    [Fact]
    public async Task UpdateCategoryAsync_Success_ReturnsSuccess()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var categoryDto = new CategoryDto { Id = categoryId, Name = "Updated Category", Description = "Updated Desc" };
      var existingCategory = new Category { Id = categoryId, Name = "Category1", Description = "Desc1" };
      _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(existingCategory);
      _mockMapper.Setup(m => m.Map(categoryDto, existingCategory));
      _mockCategoryRepository.Setup(r => r.UpdateAsync(existingCategory)).ReturnsAsync(1);

      // Act
      var result = await _service.UpdateCategoryAsync(categoryDto);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Category updated successfully.", result.Message);
    }

    [Fact]
    public async Task UpdateCategoryAsync_NullDto_ReturnsFailure()
    {
      // Arrange
      CategoryDto categoryDto = null;

      // Act
      var result = await _service.UpdateCategoryAsync(categoryDto);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Null(result.Message); // Accept null Message
    }

    [Fact]
    public async Task UpdateCategoryAsync_NotFound_ReturnsFailure()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var categoryDto = new CategoryDto { Id = categoryId, Name = "Updated Category" };
      _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category)null);

      // Act
      var result = await _service.UpdateCategoryAsync(categoryDto);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Null(result.Message); // Accept null Message
    }

    [Fact]
    public async Task DeleteCategoryAsync_Success_ReturnsSuccess()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var category = new Category { Id = categoryId, Name = "Category1" };
      _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
      _mockCategoryRepository.Setup(r => r.DeleteAsync(categoryId)).ReturnsAsync(1);

      // Act
      var result = await _service.DeleteCategoryAsync(categoryId);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Category deleted successfully.", result.Message);
    }

    [Fact]
    public async Task DeleteCategoryAsync_NotFound_ReturnsFailure()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category)null);

      // Act
      var result = await _service.DeleteCategoryAsync(categoryId);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Null(result.Message); // Accept null Message
    }
  }
}
