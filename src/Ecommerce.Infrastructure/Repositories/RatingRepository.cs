using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;

public class RatingRepository : IRatingRepository
{
  private readonly AppDbContext _context;

  public RatingRepository(AppDbContext context)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
  }
  public async Task<IEnumerable<Rating>> GetRatingsByProductIdAsync(Guid productId, int pageIndex = 1, int pageSize = 10)
  {
    return await _context.Ratings
      .AsNoTracking()
      .Where(r => r.ProductId == productId)
      .Skip((pageIndex - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync()
      .ConfigureAwait(false);
  }

  public async Task AddAsync(Rating rating)
  {
    await _context.Ratings.AddAsync(rating);
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}
