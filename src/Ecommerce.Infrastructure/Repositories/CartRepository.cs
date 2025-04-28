using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
  private readonly AppDbContext _context;

  public CartRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<CartItem>> GetCartItemsByUserAsync(string userId, int pageIndex, int pageSize)
  {

    Console.WriteLine($"Fetching cart items for user: {userId}, PageIndex: {pageIndex}, PageSize: {pageSize}");
    return await _context.CartItems
        .Where(ci => ci.UserId == userId)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
  }

  public async Task<int> AddOrUpdateCartItemAsync(string userId, Guid productId, int quantity)
  {
    var item = await _context.CartItems
        .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

    if (item == null)
    {
      item = new CartItem
      {
        UserId = userId,
        ProductId = productId,
        Quantity = quantity
      };
      _context.CartItems.Add(item);
    }
    else
    {
      item.Quantity += quantity;
      _context.CartItems.Update(item);
    }

    return await _context.SaveChangesAsync();
  }

  public async Task<int> RemoveCartItemAsync(string userId, Guid productId)
  {
    var item = await _context.CartItems
        .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

    if (item != null)
    {
      _context.CartItems.Remove(item);
      return await _context.SaveChangesAsync();
    }

    return 0;
  }

  public async Task<int> GetTotalCountByUserAsync(string userId)
  {
    return await _context.CartItems
        .CountAsync(ci => ci.UserId == userId);
  }
}
