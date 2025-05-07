using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Unitest.Repositories
{
  public class CategoryRepositoryTests
  {
    private readonly AppDbContext _context;
    private readonly ICategoryRepository _repository;

    public CategoryRepositoryTests()
    {
      var services = new ServiceCollection();
      services.AddDbContext<AppDbContext>(options =>
          options.UseInMemoryDatabase($"EcommerceTestDb_{Guid.NewGuid()}"));
      var serviceProvider = services.BuildServiceProvider();
      _context = serviceProvider.GetRequiredService<AppDbContext>();
      _repository = new CategoryRepository(_context);
    }

    private Category CreateValidCategory(Guid id, string name)
    {
      return new Category
      {
        Id = id,
        Name = name,
        Description = "Description",
        CreatedDate = DateTime.UtcNow
      };
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedCategories()
    {
      // Arrange
      var category1 = CreateValidCategory(Guid.NewGuid(), "Category1");
      var category2 = CreateValidCategory(Guid.NewGuid(), "Category2");
      _context.Categories.AddRange(category1, category2);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetAllAsync(1, 10);

      // Assert
      Assert.Equal(2, result.Count());
      Assert.Contains(result, c => c.Name == "Category1");
      Assert.Contains(result, c => c.Name == "Category2");
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsCategory()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var category = CreateValidCategory(categoryId, "Category1");
      _context.Categories.Add(category);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetByIdAsync(categoryId);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(categoryId, result.Id);
      Assert.Equal("Category1", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
      // Arrange
      var categoryId = Guid.NewGuid();

      // Act
      var result = await _repository.GetByIdAsync(categoryId);

      // Assert
      Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_CategoryExists_ReturnsTrue()
    {
      // Arrange
      var category = CreateValidCategory(Guid.NewGuid(), "Category1");
      _context.Categories.Add(category);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.ExistsAsync("Category1");

      // Assert
      Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_CategoryDoesNotExist_ReturnsFalse()
    {
      // Arrange
      // No categories added

      // Act
      var result = await _repository.ExistsAsync("Category1");

      // Assert
      Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_Success_ReturnsAffectedRows()
    {
      // Arrange
      var category = CreateValidCategory(Guid.NewGuid(), "Category1");

      // Act
      var result = await _repository.AddAsync(category);

      // Assert
      Assert.Equal(1, result);
      var savedCategory = await _context.Categories.FindAsync(category.Id);
      Assert.NotNull(savedCategory);
      Assert.Equal("Category1", savedCategory.Name);
    }

    [Fact]
    public async Task UpdateAsync_Success_ReturnsAffectedRows()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var category = CreateValidCategory(categoryId, "Category1");
      _context.Categories.Add(category);
      await _context.SaveChangesAsync();
      category.Name = "Updated Category";

      // Act
      var result = await _repository.UpdateAsync(category);

      // Assert
      Assert.Equal(1, result);
      var updatedCategory = await _context.Categories.FindAsync(categoryId);
      Assert.Equal("Updated Category", updatedCategory.Name);
    }

    [Fact]
    public async Task DeleteAsync_Success_ReturnsAffectedRows()
    {
      // Arrange
      var categoryId = Guid.NewGuid();
      var category = CreateValidCategory(categoryId, "Category1");
      _context.Categories.Add(category);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.DeleteAsync(categoryId);

      // Assert
      Assert.Equal(1, result);
      Assert.Null(await _context.Categories.FindAsync(categoryId));
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ReturnsZero()
    {
      // Arrange
      var categoryId = Guid.NewGuid();

      // Act
      var result = await _repository.DeleteAsync(categoryId);

      // Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
      // Arrange
      _context.Categories.AddRange(
          CreateValidCategory(Guid.NewGuid(), "Category1"),
          CreateValidCategory(Guid.NewGuid(), "Category2")
      );
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.CountAsync();

      // Assert
      Assert.Equal(2, result);
    }
  }
}
