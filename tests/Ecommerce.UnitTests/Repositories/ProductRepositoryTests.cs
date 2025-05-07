using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Unitest.Repositories
{
  public class ProductRepositoryTests
  {
    private readonly AppDbContext _context;
    private readonly IProductRepository _repository;

    public ProductRepositoryTests()
    {
      var services = new ServiceCollection();
      services.AddDbContext<AppDbContext>(options =>
          options.UseInMemoryDatabase($"EcommerceTestDb_{Guid.NewGuid()}"));
      var serviceProvider = services.BuildServiceProvider();
      _context = serviceProvider.GetRequiredService<AppDbContext>();
      _repository = new ProductRepository(_context);
    }

    private Product CreateValidProduct(Guid id, string name)
    {
      return new Product
      {
        Id = id,
        Name = name,
        Description = "Delicious cake",
        Price = 20.0m,
        CategoryId = Guid.NewGuid(),
        Stock = 10,
        Weight = "500g",
        Ingredients = "Flour, eggs",
        ExpirationDate = "3 days",
        StorageInstructions = "Keep cool",
        Allergens = "Gluten",
        CreatedDate = DateTime.UtcNow,
        UpdatedDate = DateTime.UtcNow,
        IsFeatured = false
      };
    }

    [Fact]
    public async Task GetFeaturedAsync_ReturnsFeaturedProducts()
    {
      // Arrange
      var product = CreateValidProduct(Guid.NewGuid(), "Cake");
      product.IsFeatured = true;
      _context.Products.Add(product);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetFeaturedAsync(1, 10);

      // Assert
      Assert.Single(result);
      Assert.NotEmpty(result);
      Assert.Equal(product.Id, result.First().Id);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsProduct()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var product = CreateValidProduct(productId, "Cake");
      _context.Products.Add(product);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetByIdAsync(productId);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(productId, result.Id);
    }

    [Fact]
    public async Task AddAsync_Success_ReturnsAffectedRows()
    {
      // Arrange
      var product = CreateValidProduct(Guid.NewGuid(), "Cake");

      // Act
      var result = await _repository.AddAsync(product);

      // Assert
      Assert.Equal(1, result);
      Assert.NotNull(await _context.Products.FindAsync(product.Id));
    }

    [Fact]
    public async Task UpdateAsync_Success_ReturnsAffectedRows()
    {
      // Arrange
      var product = CreateValidProduct(Guid.NewGuid(), "Cake");
      _context.Products.Add(product);
      await _context.SaveChangesAsync();
      product.Name = "Updated Cake";

      // Act
      var result = await _repository.UpdateAsync(product);

      // Assert
      Assert.Equal(1, result);
      var updatedProduct = await _context.Products.FindAsync(product.Id);
      Assert.Equal("Updated Cake", updatedProduct.Name);
    }

    [Fact]
    public async Task DeleteAsync_Success_ReturnsAffectedRows()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var product = CreateValidProduct(productId, "Cake");
      _context.Products.Add(product);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.DeleteAsync(productId);

      // Assert
      Assert.Equal(1, result);
      Assert.Null(await _context.Products.FindAsync(productId));
    }

    [Fact]
    public async Task ExistsAsync_ProductExists_ReturnsTrue()
    {
      // Arrange
      var product = CreateValidProduct(Guid.NewGuid(), "Cake");
      _context.Products.Add(product);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.ExistsAsync("Cake");

      // Assert
      Assert.True(result);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
      // Arrange
      _context.Products.AddRange(new[]
      {
                CreateValidProduct(Guid.NewGuid(), "Cake1"),
                CreateValidProduct(Guid.NewGuid(), "Cake2")
            });
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.CountAsync();

      // Assert
      Assert.Equal(2, result);
    }
  }
}
