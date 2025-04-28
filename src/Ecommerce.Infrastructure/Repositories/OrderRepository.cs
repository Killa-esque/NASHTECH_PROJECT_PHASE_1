using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
  private readonly AppDbContext _context;

  public OrderRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<Order> GetByIdAsync(Guid id)
  {
    var order = await _context.Orders
        .AsNoTracking()
        .FirstOrDefaultAsync(o => o.Id == id);

    if (order == null)
    {
      throw new InvalidOperationException($"Order with ID {id} not found.");
    }

    return order;
  }

  public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId)
  {
    return await _context.OrderItems
        .Where(oi => oi.OrderId == orderId)
        .ToListAsync();
  }

  public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId, int pageIndex, int pageSize)
  {
    return await _context.Orders
        .Where(o => o.UserId == userId)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
  }

  public async Task<int> CountByUserIdAsync(string userId)
  {
    return await _context.Orders
        .Where(o => o.UserId == userId)
        .CountAsync();
  }

  public async Task<IEnumerable<Order>> GetAllAsync(int pageIndex, int pageSize)
  {
    return await _context.Orders
        .OrderByDescending(o => o.CreatedDate)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
  }

  public async Task<int> CountAllAsync()
  {
    return await _context.Orders.CountAsync();
  }

  public async Task AddAsync(Order order)
  {
    await _context.Orders.AddAsync(order);
  }

  public async Task AddOrderItemsAsync(List<OrderItem> orderItems)
  {
    await _context.OrderItems.AddRangeAsync(orderItems);
  }

  public async Task UpdateAsync(Order order)
  {
    var existingOrder = await _context.Orders.FindAsync(order.Id);
    if (existingOrder == null)
    {
      throw new InvalidOperationException($"Order with ID {order.Id} not found.");
    }

    existingOrder.Status = order.Status;
    existingOrder.ShippingAddress = order.ShippingAddress;
    existingOrder.Note = order.Note;
    existingOrder.UpdatedDate = DateTime.UtcNow;
  }


  public async Task<int> SaveChangesAsync()
  {
    return await _context.SaveChangesAsync();
  }
}

