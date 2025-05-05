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
    var query = from rating in _context.Ratings
                join user in _context.Users on rating.UserId equals user.Id
                where rating.ProductId == productId
                select new Rating
                {
                  Id = rating.Id,
                  ProductId = rating.ProductId,
                  UserId = rating.UserId,
                  RatingValue = rating.RatingValue,
                  Comment = rating.Comment,
                  CreatedDate = rating.CreatedDate,
                  // Since Rating entity doesn't have UserName, we'll handle it in the service layer
                };

    return await query
        .AsNoTracking()
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

  public async Task<string> GetUserNameByIdAsync(string userId)
  {
    var user = await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == userId)
        .ConfigureAwait(false);

    return user?.UserName ?? "Unknown User";
  }

  public async Task<int> CountRatingsByProductIdAsync(Guid productId)
  {
    return await _context.Ratings
        .AsNoTracking()
        .CountAsync(r => r.ProductId == productId)
        .ConfigureAwait(false);
  }

}
