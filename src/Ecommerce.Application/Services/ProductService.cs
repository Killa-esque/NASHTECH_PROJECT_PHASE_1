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
  private readonly IMapper _mapper;

  public ProductService(IProductRepository productRepository, IMapper mapper)
  {
    _productRepository = productRepository;
    _mapper = mapper;
  }

  public async Task<Result> AddProductAsync(ProductDto productDto)
  {
    if (productDto == null)
      return Result.Failure("Product cannot be null.");

    var product = _mapper.Map<Product>(productDto);
    var exists = await _productRepository.ExistsAsync(product.Name);
    if (exists)
      return Result.Failure("Product already exists.");

    var affectedRows = await _productRepository.AddAsync(product);
    if (affectedRows == 0)
      return Result.Failure("Failed to add product.");

    return Result.Success("Product added successfully.");
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
    var exists = await _productRepository.ExistsAsync(product.Name);
    if (!exists)
      return Result.Failure("Product not found.");

    var affectedRows = await _productRepository.UpdateAsync(product);
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
}
