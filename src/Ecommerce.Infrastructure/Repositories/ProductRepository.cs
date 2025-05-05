using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
  private readonly AppDbContext _context;

  public ProductRepository(AppDbContext context)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
  }

  public async Task<IEnumerable<Product>> GetFeaturedAsync(int pageIndex, int pageSize)
  {
    return await _context.Products
        .AsNoTracking()
        .Where(p => p.IsFeatured)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync()
        .ConfigureAwait(false);
  }

  public async Task<IEnumerable<Product>> GetAllAsync(int pageIndex, int pageSize)
  {
    return await _context.Products
      .AsNoTracking()
      .Skip((pageIndex - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync()
      .ConfigureAwait(false);
  }

  public async Task<Product?> GetByIdAsync(Guid id)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentException("Product ID cannot be empty.", nameof(id));
    }

    return await _context.Products
      .AsNoTracking()
      .FirstOrDefaultAsync(p => p.Id == id)
      .ConfigureAwait(false);
  }

  public async Task<IEnumerable<Product>> GetByIdsAsync(List<Guid> ids)
  {
    if (ids == null || !ids.Any())
    {
      throw new ArgumentException("Product IDs cannot be null or empty.", nameof(ids));
    }

    return await _context.Products
      .AsNoTracking()
      .Where(p => ids.Contains(p.Id))
      .ToListAsync()
      .ConfigureAwait(false);
  }

  public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, int pageIndex, int pageSize)
  {
    return await _context.Products
      .AsNoTracking()
      .Where(p => p.CategoryId == categoryId)
      .Skip((pageIndex - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync()
      .ConfigureAwait(false);
  }

  public async Task<IEnumerable<ProductImage>> GetProductImagesAsync(Guid productId)
  {
    if (productId == Guid.Empty)
    {
      throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
    }

    return await _context.ProductImages
      .AsNoTracking()
      .Where(pi => pi.ProductId == productId)
      .OrderByDescending(pi => pi.IsPrimary)
      .ToListAsync()
      .ConfigureAwait(false);
  }

  public Task<int> AddAsync(Product product)
  {
    if (product == null)
    {
      throw new ArgumentNullException(nameof(product), "Product cannot be null.");
    }

    _context.Products.Add(product);
    return _context.SaveChangesAsync();
  }
  public Task<int> UpdateAsync(Product product)
  {
    if (product == null)
    {
      throw new ArgumentNullException(nameof(product), "Product cannot be null.");
    }

    _context.Products.Update(product);
    return _context.SaveChangesAsync();
  }
  public Task<int> DeleteAsync(Guid id)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentException("Product ID cannot be empty.", nameof(id));
    }

    var product = _context.Products.Find(id);
    if (product == null)
    {
      throw new InvalidOperationException($"Product with ID {id} not found.");
    }

    _context.Products.Remove(product);
    return _context.SaveChangesAsync();
  }
  public Task<bool> ExistsAsync(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Product name cannot be null or empty.", nameof(name));
    }

    return _context.Products.AsNoTracking().AnyAsync(p => p.Name == name);
  }
  public Task<int> CountAsync()
  {
    return _context.Products.CountAsync();
  }
  public Task<int> CountByCategoryAsync(Guid categoryId)
  {
    if (categoryId == Guid.Empty)
    {
      throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
    }

    return _context.Products.CountAsync(p => p.CategoryId == categoryId);
  }
  public Task<int> CountFeaturedProductsAsync()
  {
    return _context.Products.CountAsync(p => p.IsFeatured);
  }
}
