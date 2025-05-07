using AutoMapper;
using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Application.Services;
using Ecommerce.Domain.Entities;
using Ecommerce.Shared.DTOs;
using Moq;

namespace Ecommerce.Unitest.Services
{
  public class ProductServiceTests
  {
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ISupabaseStorageService> _mockStorageService;
    private readonly Mock<IProductImageRepository> _mockImageRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
      _mockProductRepository = new Mock<IProductRepository>();
      _mockStorageService = new Mock<ISupabaseStorageService>();
      _mockImageRepository = new Mock<IProductImageRepository>();
      _mockMapper = new Mock<IMapper>();
      _service = new ProductService(_mockProductRepository.Object, _mockMapper.Object,
                                   _mockStorageService.Object, _mockImageRepository.Object);
    }

    [Fact]
    public async Task AddProductAsync_Success_ReturnsProductId()
    {
      // Arrange
      var productDto = new ProductDto
      {
        Name = "Cake",
        Description = "Delicious cake",
        Price = 20.0m,
        CategoryId = Guid.NewGuid(),
        Stock = 10,
        Weight = "500g",
        Ingredients = "Flour, eggs",
        ExpirationDate = "3 days",
        StorageInstructions = "Keep cool",
        Allergens = "Gluten"
      };
      var product = new Product
      {
        Id = Guid.NewGuid(),
        Name = "Cake",
        Description = "Delicious cake",
        Price = 20.0m,
        CategoryId = productDto.CategoryId,
        Stock = 10,
        Weight = "500g",
        Ingredients = "Flour, eggs",
        ExpirationDate = "3 days",
        StorageInstructions = "Keep cool",
        Allergens = "Gluten"
      };
      _mockMapper.Setup(m => m.Map<Product>(productDto)).Returns(product);
      _mockProductRepository.Setup(r => r.ExistsAsync(product.Name)).ReturnsAsync(false);
      _mockProductRepository.Setup(r => r.AddAsync(product)).ReturnsAsync(1);

      // Act
      var result = await _service.AddProductAsync(productDto);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Product added successfully.", result.Message);
      Assert.NotEqual(Guid.Empty, result.Data);
    }

#pragma warning disable CS8600, CS8604 // Disable nullable warnings for null test
    [Fact]
    public async Task AddProductAsync_NullDto_ReturnsFailure()
    {
      // Arrange
      ProductDto productDto = null;

      // Act
      var result = await _service.AddProductAsync(productDto);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Null(result.Message);
    }
#pragma warning restore CS8600, CS8604

    [Fact]
    public async Task DeleteProductAsync_Success_ReturnsSuccess()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var product = new Product
      {
        Id = productId,
        Name = "Cake",
        Description = "Delicious cake",
        Price = 20.0m,
        CategoryId = Guid.NewGuid(),
        Stock = 10,
        Weight = "500g",
        Ingredients = "Flour, eggs",
        ExpirationDate = "3 days",
        StorageInstructions = "Keep cool",
        Allergens = "Gluten"
      };
      _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
      _mockProductRepository.Setup(r => r.DeleteAsync(productId)).ReturnsAsync(1);

      // Act
      var result = await _service.DeleteProductAsync(productId);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Product deleted successfully.", result.Message);
    }

    [Fact]
    public async Task GetAllProductsAsync_Success_ReturnsPagedResult()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Cake",
                    Description = "Delicious cake",
                    Price = 20.0m,
                    CategoryId = Guid.NewGuid(),
                    Stock = 10,
                    Weight = "500g",
                    Ingredients = "Flour, eggs",
                    ExpirationDate = "3 days",
                    StorageInstructions = "Keep cool",
                    Allergens = "Gluten"
                }
            };
      var productDtos = new List<ProductDto>
            {
                new ProductDto
                {
                    Id = products[0].Id,
                    Name = "Cake",
                    Description = "Delicious cake",
                    Price = 20.0m,
                    CategoryId = products[0].CategoryId,
                    Stock = 10,
                    Weight = "500g",
                    Ingredients = "Flour, eggs",
                    ExpirationDate = "3 days",
                    StorageInstructions = "Keep cool",
                    Allergens = "Gluten"
                }
            };
      _mockProductRepository.Setup(r => r.GetAllAsync(pageIndex, pageSize)).ReturnsAsync(products);
      _mockProductRepository.Setup(r => r.CountAsync()).ReturnsAsync(1);
      _mockProductRepository.Setup(r => r.GetProductImagesAsync(It.IsAny<Guid>())).ReturnsAsync(new List<ProductImage>());
      _mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(productDtos[0]);

      // Act
      var result = await _service.GetAllProductsAsync(pageIndex, pageSize);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal(1, result.Data.TotalCount);
      Assert.NotNull(result.Data.Items);
      Assert.Single(result.Data.Items);
    }

    [Fact]
    public async Task GetProductByIdAsync_Success_ReturnsProductDto()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var product = new Product
      {
        Id = productId,
        Name = "Cake",
        Description = "Delicious cake",
        Price = 20.0m,
        CategoryId = Guid.NewGuid(),
        Stock = 10,
        Weight = "500g",
        Ingredients = "Flour, eggs",
        ExpirationDate = "3 days",
        StorageInstructions = "Keep cool",
        Allergens = "Gluten"
      };
      var productDto = new ProductDto
      {
        Id = productId,
        Name = "Cake",
        Description = "Delicious cake",
        Price = 20.0m,
        CategoryId = product.CategoryId,
        Stock = 10,
        Weight = "500g",
        Ingredients = "Flour, eggs",
        ExpirationDate = "3 days",
        StorageInstructions = "Keep cool",
        Allergens = "Gluten"
      };
      _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
      _mockProductRepository.Setup(r => r.GetProductImagesAsync(productId)).ReturnsAsync(new List<ProductImage>());
      _mockMapper.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

      // Act
      var result = await _service.GetProductByIdAsync(productId);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal(productDto, result.Data);
      Assert.Equal("Product retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task UpdateProductAsync_Success_ReturnsSuccess()
    {
      // Arrange
      var productDto = new ProductDto
      {
        Id = Guid.NewGuid(),
        Name = "Cake",
        Description = "Delicious cake",
        Price = 20.0m,
        CategoryId = Guid.NewGuid(),
        Stock = 10,
        Weight = "500g",
        Ingredients = "Flour, eggs",
        ExpirationDate = "3 days",
        StorageInstructions = "Keep cool",
        Allergens = "Gluten"
      };
      var product = new Product
      {
        Id = productDto.Id,
        Name = "Cake",
        Description = "Delicious cake",
        Price = 20.0m,
        CategoryId = productDto.CategoryId,
        Stock = 10,
        Weight = "500g",
        Ingredients = "Flour, eggs",
        ExpirationDate = "3 days",
        StorageInstructions = "Keep cool",
        Allergens = "Gluten"
      };
      _mockMapper.Setup(m => m.Map<Product>(productDto)).Returns(product);
      _mockProductRepository.Setup(r => r.ExistsAsync(product.Name)).ReturnsAsync(true);
      _mockProductRepository.Setup(r => r.UpdateAsync(product)).ReturnsAsync(1);

      // Act
      var result = await _service.UpdateProductAsync(productDto);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Product updated successfully.", result.Message);
    }

    [Fact]
    public async Task SetProductFeaturedAsync_Success_ReturnsSuccess()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var product = new Product
      {
        Id = productId,
        Name = "Cake",
        Description = "Delicious cake",
        Price = 20.0m,
        CategoryId = Guid.NewGuid(),
        Stock = 10,
        Weight = "500g",
        Ingredients = "Flour, eggs",
        ExpirationDate = "3 days",
        StorageInstructions = "Keep cool",
        Allergens = "Gluten",
        IsFeatured = false
      };
      _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
      _mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(1);

      // Act
      var result = await _service.SetProductFeaturedAsync(productId, true);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Product updated successfully.", result.Message);
    }

    [Fact]
    public async Task AddProductImagesAsync_Success_ReturnsSuccess()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var files = new List<(Stream, string, string)> { (new MemoryStream(), "test.jpg", "image/jpeg") };
      var urls = new List<string> { "http://example.com/test.jpg" };
      _mockStorageService.Setup(s => s.UploadProductImagesAsync(files, productId)).ReturnsAsync(urls);
      _mockImageRepository.Setup(r => r.AddRangeAsync(It.IsAny<List<ProductImage>>())).Returns(Task.CompletedTask);

      // Act
      var result = await _service.AddProductImagesAsync(productId, files);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Images uploaded and saved.", result.Message);
    }

    [Fact]
    public async Task DeleteProductImageAsync_Success_ReturnsSuccess()
    {
      // Arrange
      var productId = Guid.NewGuid();
      var imageUrl = "http://example.com/test.jpg";
      var productImage = new ProductImage { Id = Guid.NewGuid(), ProductId = productId, ImageUrl = imageUrl };
      _mockImageRepository.Setup(r => r.GetByProductIdAndUrlAsync(productId, imageUrl)).ReturnsAsync(productImage);
      _mockStorageService.Setup(s => s.DeleteProductImageAsync(productId, "test.jpg")).ReturnsAsync(true);
      _mockImageRepository.Setup(r => r.DeleteAsync(productImage.Id)).Returns(Task.CompletedTask);

      // Act
      var result = await _service.DeleteProductImageAsync(productId, imageUrl);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Image deleted.", result.Message);
    }
  }
}
