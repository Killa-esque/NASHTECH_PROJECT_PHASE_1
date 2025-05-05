using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services;

public class ProductService : IProductService
{
  private readonly IProductApiClient _productApiClient;
  private readonly ICategoryService _categoryService;
  private readonly ILogger<ProductService> _logger;

  public ProductService(IProductApiClient productApiClient, ICategoryService categoryService, ILogger<ProductService> logger)
  {
    _productApiClient = productApiClient;
    _categoryService = categoryService;
    _logger = logger;
  }

  public async Task<PagedResult<ProductViewModel>> GetAllProductsAsync(int pageIndex, int pageSize)
  {
    var productDtos = await _productApiClient.GetAllProductsAsync(pageIndex, pageSize);
    var products = await MapToProductViewModels(productDtos.Items.ToList());
    return PagedResult<ProductViewModel>.Create(products, productDtos.TotalCount, pageIndex, pageSize);
  }

  public async Task<PagedResult<ProductViewModel>> GetFeaturedProductsAsync(int pageIndex, int pageSize)
  {
    var pagedDto = await _productApiClient.GetFeaturedProductsAsync(pageIndex, pageSize);
    var items = await MapToProductViewModels(pagedDto.Items.ToList());
    return PagedResult<ProductViewModel>.Create(items, pagedDto.TotalCount, pageIndex, pageSize);
  }

  public async Task<PagedResult<ProductViewModel>> GetProductsForCategoryPageAsync(Guid categoryId, int pageIndex, int pageSize)
  {
    var productDtos = await _productApiClient.GetProductsForCategoryPageAsync(categoryId, pageIndex, pageSize);
    var products = await MapToProductViewModels(productDtos.Items.ToList());
    return PagedResult<ProductViewModel>.Create(products, productDtos.TotalCount, pageIndex, pageSize);
  }

  public async Task<ProductViewModel> GetProductDetailsAsync(Guid productId)
  {
    var productDto = await _productApiClient.GetProductDetailsAsync(productId);
    if (productDto == null)
    {
      _logger.LogWarning("Product {ProductId} not found in API", productId);
      return null;
    }

    var result = await MapToProductViewModel(productDto);
    return result;
  }

  private async Task<ProductViewModel> MapToProductViewModel(ProductDto dto)
  {
    CategoryViewModel category = null;
    if (dto.CategoryId != Guid.Empty)
    {
      try
      {
        category = await _categoryService.GetCategoryByIdAsync(dto.CategoryId);
      }
      catch (Exception ex)
      {
        _logger.LogWarning("Không tìm thấy danh mục với CategoryId={CategoryId}: {Error}", dto.CategoryId, ex.Message);
      }
    }
    else
    {
      _logger.LogWarning("CategoryId là Guid.Empty cho sản phẩm {ProductId}.", dto.Id);
    }

    return new ProductViewModel
    {
      Id = dto.Id,
      Name = dto.Name,
      Description = dto.Description,
      Price = dto.Price,
      ImageUrl = dto.ImageUrls ?? new List<string>(),
      CategoryId = dto.CategoryId,
      Weight = dto.Weight,
      Ingredients = dto.Ingredients,
      ExpirationDate = dto.ExpirationDate,
      StorageInstructions = dto.StorageInstructions,
      Allergens = dto.Allergens
    };
  }

  private async Task<List<ProductViewModel>> MapToProductViewModels(List<ProductDto> dtos)
  {
    var products = new List<ProductViewModel>();
    foreach (var dto in dtos)
    {
      products.Add(await MapToProductViewModel(dto));
    }
    return products;
  }
}
