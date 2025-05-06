using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;


public class ProductImageRepository : IProductImageRepository
{
  private readonly AppDbContext _context;

  public ProductImageRepository(AppDbContext context) => _context = context;

  public async Task AddRangeAsync(IEnumerable<ProductImage> images)
  {
    _context.ProductImages.AddRange(images);
    await _context.SaveChangesAsync();
  }

  public async Task<ProductImage?> GetByProductIdAndUrlAsync(Guid productId, string imageUrl)
  {
    return await _context.ProductImages.FirstOrDefaultAsync(p => p.ProductId == productId && p.ImageUrl == imageUrl);
  }

  public async Task DeleteAsync(Guid id)
  {
    var entity = await _context.ProductImages.FindAsync(id);
    if (entity != null)
    {
      _context.ProductImages.Remove(entity);
      await _context.SaveChangesAsync();
    }
  }
}
