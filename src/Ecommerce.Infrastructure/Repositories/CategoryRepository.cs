using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
  private readonly AppDbContext _context;

  public CategoryRepository(AppDbContext context)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
  }

  public async Task<IEnumerable<Category>> GetAllAsync(int pageIndex, int pageSize)
  {
    return await _context.Categories
      .AsNoTracking()
      .Skip((pageIndex - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync();
  }

  public async Task<bool> ExistsAsync(string name)
  {
    return await _context.Categories.AsNoTracking().AnyAsync(c => c.Name == name);
  }

  public async Task<Category?> GetByIdAsync(Guid id)
  {
    return await _context.Categories.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
  }

  public async Task<int> AddAsync(Category category)
  {
    _context.Categories.Add(category);
    return await _context.SaveChangesAsync();
  }

  public async Task<int> UpdateAsync(Category category)
  {
    _context.Categories.Update(category);
    return await _context.SaveChangesAsync();
  }

  public async Task<int> DeleteAsync(Guid id)
  {
    var category = await _context.Categories.FindAsync(id);
    if (category == null) return 0;

    _context.Categories.Remove(category);
    return await _context.SaveChangesAsync();
  }

  public async Task<int> CountAsync()
  {
    return await _context.Categories.CountAsync();
  }
}
