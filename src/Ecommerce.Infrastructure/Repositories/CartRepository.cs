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
    _context = context ?? throw new ArgumentNullException(nameof(context));
  }

  public async Task<IEnumerable<CartItem>> GetCartItemsByUserAsync(string userId, int pageIndex, int pageSize)
  {
    return await _context.CartItems
        .AsNoTracking()
        .Where(ci => ci.UserId == userId)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync()
        .ConfigureAwait(false);
  }

  public async Task<int> GetTotalCountByUserAsync(string userId)
  {
    return await _context.CartItems
        .CountAsync(ci => ci.UserId == userId)
        .ConfigureAwait(false);
  }
  public async Task<int> AddOrUpdateCartItemAsync(string userId, Guid productId, int quantity, bool isUpdate = false)
  {
    var cartItem = await _context.CartItems
        .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

    if (cartItem == null)
    {
      cartItem = new CartItem
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        ProductId = productId,
        Quantity = quantity,
        AddedDate = DateTime.UtcNow
      };
      await _context.CartItems.AddAsync(cartItem);
    }
    else
    {
      if (isUpdate)
      {
        cartItem.Quantity = quantity; // Update quantity
      }
      else
      {
        cartItem.Quantity += quantity; // Add to existing quantity
      }
    }

    return await _context.SaveChangesAsync();
  }

  public async Task<int> RemoveCartItemAsync(string userId, Guid productId)
  {
    var cartItem = await _context.CartItems
        .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

    if (cartItem == null) return 0;

    _context.CartItems.Remove(cartItem);
    return await _context.SaveChangesAsync();
  }

  public async Task<int> ClearCartAsync(string userId)
  {
    var cartItems = await _context.CartItems
        .Where(ci => ci.UserId == userId)
        .ToListAsync();

    _context.CartItems.RemoveRange(cartItems);
    return await _context.SaveChangesAsync();
  }
}
