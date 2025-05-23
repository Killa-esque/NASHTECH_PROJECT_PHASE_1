using AutoMapper;
using Ecommerce.Shared.DTOs;
using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Common;

namespace Ecommerce.Application.Services;

public class ProductService : IProductService
{
  private readonly IProductRepository _productRepository;
  private readonly ISupabaseStorageService _storageService;
  private readonly IProductImageRepository _imageRepository;
  private readonly IMapper _mapper;

  public ProductService(IProductRepository productRepository, IMapper mapper,
                       ISupabaseStorageService storageService, IProductImageRepository imageRepository)
  {
    _productRepository = productRepository;
    _mapper = mapper;
    _storageService = storageService;
    _imageRepository = imageRepository;
  }

  public async Task<Result<Guid>> AddProductAsync(ProductDto productDto)
  {
    if (productDto == null)
      return Result.Failure<Guid>("Product cannot be null.");

    var product = _mapper.Map<Product>(productDto);
    var exists = await _productRepository.ExistsAsync(product.Name);
    if (exists)
      return Result.Failure<Guid>("Product already exists.");

    product.Id = Guid.NewGuid();
    var affectedRows = await _productRepository.AddAsync(product);
    if (affectedRows == 0)
      return Result.Failure<Guid>("Failed to add product.");

    return Result.Success(product.Id, "Product added successfully.");
  }

  public async Task<Result> DeleteProductAsync(Guid id)
  {
    if (id == Guid.Empty)
      return Result.Failure("Invalid product ID.");

    var product = await _productRepository.GetByIdAsync(id);
    if (product == null)
      return Result.Failure("Product does not exist.");

    var affectedRows = await _productRepository.DeleteAsync(id);
    if (affectedRows == 0)
      return Result.Failure("Failed to delete product.");

    return Result.Success("Product deleted successfully.");
  }

  public async Task<Result<PagedResult<ProductDto>>> GetAllProductsAsync(int pageIndex, int pageSize)
  {
    var products = await _productRepository.GetAllAsync(pageIndex, pageSize);
    var totalCount = await _productRepository.CountAsync();

    var productDtos = new List<ProductDto>();
    foreach (var product in products)
    {
      var productDto = _mapper.Map<ProductDto>(product);
      var images = await _productRepository.GetProductImagesAsync(product.Id);
      productDto.ImageUrls = images.Select(i => i.ImageUrl).ToList();
      productDtos.Add(productDto);
    }

    var pagedResult = PagedResult<ProductDto>.Create(productDtos, totalCount, pageIndex, pageSize);
    return Result.Success(pagedResult);
  }

  public async Task<Result<PagedResult<ProductDto>>> GetFeaturedProductsAsync(int pageIndex, int pageSize)
  {
    var products = await _productRepository.GetFeaturedAsync(pageIndex, pageSize);
    if (products == null)
      return Result.Failure<PagedResult<ProductDto>>("Failed to load featured products.");

    var totalCount = await _productRepository.CountFeaturedProductsAsync();
    var productDtos = new List<ProductDto>();
    foreach (var product in products)
    {
      var productDto = _mapper.Map<ProductDto>(product);
      var images = await _productRepository.GetProductImagesAsync(product.Id);
      productDto.ImageUrls = images.Select(i => i.ImageUrl).ToList();
      productDtos.Add(productDto);
    }

    var pagedResult = PagedResult<ProductDto>.Create(productDtos, totalCount, pageIndex, pageSize);
    return Result.Success(pagedResult, "Featured products retrieved successfully.");
  }

  public async Task<Result<ProductDto>> GetProductByIdAsync(Guid id)
  {
    if (id == Guid.Empty)
      return Result.Failure<ProductDto>("Invalid product ID.");

    var product = await _productRepository.GetByIdAsync(id);
    if (product == null)
      return Result.Failure<ProductDto>("Product not found.");

    var productDto = _mapper.Map<ProductDto>(product);
    var images = await _productRepository.GetProductImagesAsync(id);
    productDto.ImageUrls = images.Select(i => i.ImageUrl).ToList();

    return Result.Success(productDto, "Product retrieved successfully.");
  }

  public async Task<Result<PagedResult<ProductDto>>> GetProductsByCategoryAsync(Guid categoryId, int pageIndex, int pageSize)
  {
    if (categoryId == Guid.Empty)
      return Result.Failure<PagedResult<ProductDto>>("Invalid category ID.");

    var products = await _productRepository.GetByCategoryIdAsync(categoryId, pageIndex, pageSize);
    var totalCount = await _productRepository.CountByCategoryAsync(categoryId);

    var productDtos = new List<ProductDto>();
    foreach (var product in products)
    {
      var productDto = _mapper.Map<ProductDto>(product);
      var images = await _productRepository.GetProductImagesAsync(product.Id);
      productDto.ImageUrls = images.Select(i => i.ImageUrl).ToList();
      productDtos.Add(productDto);
    }

    var pagedResult = PagedResult<ProductDto>.Create(productDtos, totalCount, pageIndex, pageSize);
    return Result.Success(pagedResult, "Products by category retrieved successfully.");
  }

  public async Task<Result> UpdateProductAsync(ProductDto productDto)
  {
    if (productDto == null)
      return Result.Failure("Product cannot be null.");

    var product = _mapper.Map<Product>(productDto);
    var exists = await _productRepository.ExistsAsync(product.Name, product.Id);
    if (exists)
      return Result.Failure("Product with this name already exists.");

    var currentProduct = await _productRepository.GetByIdAsync(product.Id);
    if (currentProduct == null)
      return Result.Failure("Product not found.");

    // Cập nhật các trường
    currentProduct.Name = product.Name;
    currentProduct.Description = product.Description;
    currentProduct.Price = product.Price;
    currentProduct.CategoryId = product.CategoryId;
    currentProduct.Stock = product.Stock;
    currentProduct.Weight = product.Weight;
    currentProduct.Ingredients = product.Ingredients;
    currentProduct.ExpirationDate = product.ExpirationDate;
    currentProduct.StorageInstructions = product.StorageInstructions;
    currentProduct.Allergens = product.Allergens;
    currentProduct.UpdatedDate = DateTime.UtcNow;

    var affectedRows = await _productRepository.UpdateAsync(currentProduct);
    if (affectedRows == 0)
      return Result.Failure("Failed to update product.");

    return Result.Success("Product updated successfully.");
  }

  public async Task<Result> SetProductFeaturedAsync(Guid id, bool isFeatured)
  {
    if (id == Guid.Empty)
      return Result.Failure("Invalid product ID.");

    var product = await _productRepository.GetByIdAsync(id);
    if (product == null)
      return Result.Failure("Product not found.");

    product.IsFeatured = isFeatured;
    var affectedRows = await _productRepository.UpdateAsync(product);
    if (affectedRows == 0)
      return Result.Failure("Failed to update product.");

    return Result.Success("Product updated successfully.");
  }

  public async Task<Result> AddProductImagesAsync(Guid productId, List<(Stream fileStream, string fileName, string contentType)> files)
  {
    var urls = await _storageService.UploadProductImagesAsync(files, productId);

    var imageEntities = urls.Select(url => new ProductImage
    {
      Id = Guid.NewGuid(),
      ProductId = productId,
      ImageUrl = url,
      IsPrimary = false,
      CreatedDate = DateTime.UtcNow
    }).ToList();

    await _imageRepository.AddRangeAsync(imageEntities);
    return Result.Success("Images uploaded and saved.");
  }

  public async Task<Result> DeleteProductImageAsync(Guid productId, string imageUrl)
  {
    var image = await _imageRepository.GetByProductIdAndUrlAsync(productId, imageUrl);
    if (image == null) return Result.Failure("Image not found.");

    var fileName = Path.GetFileName(new Uri(image.ImageUrl).LocalPath);
    var success = await _storageService.DeleteProductImageAsync(productId, fileName);
    if (!success) return Result.Failure("Failed to delete image from storage.");

    await _imageRepository.DeleteAsync(image.Id);
    return Result.Success("Image deleted.");
  }
}
