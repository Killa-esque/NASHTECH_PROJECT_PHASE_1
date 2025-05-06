using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces.Repositories;

public interface IOrderRepository
{
  Task<Order> GetByIdAsync(Guid id);
  Task<IEnumerable<Order>> GetByUserIdAsync(string userId, int pageIndex, int pageSize);
  Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId);
  Task<int> CountByUserIdAsync(string userId);
  Task AddAsync(Order order);
  Task UpdateAsync(Order order);
  Task AddOrderItemsAsync(List<OrderItem> orderItems);
  Task<IEnumerable<Order>> GetAllAsync(int pageIndex, int pageSize);
  Task<int> CountAllAsync();
  Task<int> SaveChangesAsync();
  Task<IEnumerable<Order>> GetByCustomerIdAsync(string customerId, int pageIndex, int pageSize);
  Task<int> CountByCustomerIdAsync(string customerId);
  Task<bool> DeleteByUserIdAsync(string userId);
}
