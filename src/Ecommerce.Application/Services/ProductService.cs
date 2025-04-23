using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.DTOs;
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
    product.ImageUrl ??= "";

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
      return Result.Failure("Product not exists.");

    var affectedRows = await _productRepository.DeleteAsync(id);

    if (affectedRows == 0)
      return Result.Failure("Failed to delete product.");

    return Result.Success("Product deleted successfully.");
  }

  public async Task<Result<PagedResult<ProductDto>>> GetAllProductsAsync(int pageIndex, int pageSize)
  {
    var products = await _productRepository.GetAllAsync(pageIndex, pageSize);

    var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

    var totalCount = await _productRepository.CountAsync();

    var pagedResult = PagedResult<ProductDto>.Create(productDtos, totalCount, pageIndex, pageSize);

    return Result.Success(pagedResult);
  }

  public async Task<Result<PagedResult<ProductDto>>> GetFeaturedProductsAsync(int pageIndex, int pageSize)
  {
    var products = await _productRepository.GetFeaturedAsync(pageIndex, pageSize);
    if (products == null)
      return Result.Failure<PagedResult<ProductDto>>("Failed to load featured products.");

    var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
    var totalCount = await _productRepository.CountFeaturedProductsAsync();
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
    return Result.Success(productDto, "Product retrieved successfully.");
  }

  public async Task<Result<PagedResult<ProductDto>>> GetProductsByCategoryAsync(Guid categoryId, int pageIndex, int pageSize)
  {
    if (categoryId == Guid.Empty)
      return Result.Failure<PagedResult<ProductDto>>("Invalid category ID.");

    var products = await _productRepository.GetByCategoryIdAsync(categoryId, pageIndex, pageSize);
    var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

    var totalCount = await _productRepository.CountByCategoryAsync(categoryId);

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

