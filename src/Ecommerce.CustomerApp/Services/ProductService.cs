using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services;

public class ProductService : IProductService
{
  private readonly IProductApiClient _productApiClient;

  public ProductService(IProductApiClient productApiClient)
  {
    _productApiClient = productApiClient;
  }

  public async Task<ProductViewModel> GetProductDetailsAsync(Guid id)
  {
    var product = await _productApiClient.GetProductByIdAsync(id);
    if (product == null)
    {
      throw new Exception($"Product with ID {id} not found.");
    }
    return product;
  }

  public async Task<List<ProductViewModel>> GetProductsForCategoryPageAsync(Guid categoryId, int pageIndex, int pageSize)
  {
    var products = await _productApiClient.GetProductsByCategoryAsync(categoryId, pageIndex, pageSize);
    return products;
  }
}
