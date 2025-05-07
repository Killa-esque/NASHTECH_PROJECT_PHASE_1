using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.UnitTests.Repositories
{
  public class OrderRepositoryTests
  {
    private readonly AppDbContext _context;
    private readonly OrderRepository _repository;
    private readonly string _userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";

    public OrderRepositoryTests()
    {
      var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;
      _context = new AppDbContext(options);
      _repository = new OrderRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsOrder()
    {
      // Arrange
      var order = new Order
      {
        Id = Guid.NewGuid(),
        UserId = _userId,
        TotalAmount = 100m,
        OrderCode = "ORD-123",
        PaymentMethod = "CreditCard",
        ShippingAddress = "123 Street",
        Note = "",
        Status = OrderStatus.Pending,
        CreatedDate = DateTime.UtcNow
      };
      _context.Orders.Add(order);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetByIdAsync(order.Id);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(order.Id, result.Id);
      Assert.Equal(100m, result.TotalAmount);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsException()
    {
      // Arrange
      var invalidId = Guid.NewGuid();

      // Act & Assert
      await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.GetByIdAsync(invalidId));
    }

    [Fact]
    public async Task GetByUserIdAsync_Success_ReturnsOrders()
    {
      // Arrange
      var orders = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,
                    TotalAmount = 100m,
                    OrderCode = "ORD-123",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,
                    TotalAmount = 200m,
                    OrderCode = "ORD-124",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                }
            };
      _context.Orders.AddRange(orders);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetByUserIdAsync(_userId, 1, 10);

      // Assert
      Assert.Equal(2, result.Count());
      Assert.Contains(result, o => o.TotalAmount == 100m);
      Assert.Contains(result, o => o.TotalAmount == 200m);
    }

    [Fact]
    public async Task GetOrderItemsByOrderIdAsync_Success_ReturnsOrderItems()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var orderItems = new List<OrderItem>
            {
                new OrderItem { Id = Guid.NewGuid(), OrderId = orderId, ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 50m }
            };
      _context.OrderItems.AddRange(orderItems);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetOrderItemsByOrderIdAsync(orderId);

      // Assert
      Assert.Single(result);
      Assert.Equal(2, result.First().Quantity);
      Assert.Equal(50m, result.First().UnitPrice);
    }

    [Fact]
    public async Task CountByUserIdAsync_Success_ReturnsCount()
    {
      // Arrange
      _context.Orders.AddRange(new[]
      {
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,
                    OrderCode = "ORD-123",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,
                    OrderCode = "ORD-124",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                }
            });
      await _context.SaveChangesAsync();

      // Act
      var count = await _repository.CountByUserIdAsync(_userId);

      // Assert
      Assert.Equal(2, count);
    }

    [Fact]
    public async Task AddAsync_Success_SavesOrder()
    {
      // Arrange
      var order = new Order
      {
        Id = Guid.NewGuid(),
        UserId = _userId,
        TotalAmount = 100m,
        OrderCode = "ORD-123",
        PaymentMethod = "CreditCard",
        ShippingAddress = "123 Street",
        Note = "",
        Status = OrderStatus.Pending,
        CreatedDate = DateTime.UtcNow
      };

      // Act
      await _repository.AddAsync(order);
      await _repository.SaveChangesAsync();

      // Assert
      var savedOrder = await _context.Orders.FindAsync(order.Id);
      Assert.NotNull(savedOrder);
      Assert.Equal(100m, savedOrder!.TotalAmount);
    }

    [Fact]
    public async Task UpdateAsync_Success_UpdatesOrder()
    {
      // Arrange
      var order = new Order
      {
        Id = Guid.NewGuid(),
        UserId = _userId,
        Status = OrderStatus.Pending,
        OrderCode = "ORD-123",
        PaymentMethod = "CreditCard",
        ShippingAddress = "123 Street",
        Note = "",
        CreatedDate = DateTime.UtcNow
      };
      _context.Orders.Add(order);
      await _context.SaveChangesAsync();

      order.Status = OrderStatus.Shipped;

      // Act
      await _repository.UpdateAsync(order);
      await _context.SaveChangesAsync();

      // Assert
      var updatedOrder = await _context.Orders.FindAsync(order.Id);
      Assert.Equal(OrderStatus.Shipped, updatedOrder!.Status);
    }

    [Fact]
    public async Task UpdateAsync_OrderNotFound_ThrowsException()
    {
      // Arrange
      var order = new Order
      {
        Id = Guid.NewGuid(),
        UserId = _userId,
        OrderCode = "ORD-123",
        PaymentMethod = "CreditCard",
        ShippingAddress = "123 Street",
        Note = ""
      };

      // Act & Assert
      await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.UpdateAsync(order));
    }

    [Fact]
    public async Task AddOrderItemsAsync_Success_SavesOrderItems()
    {
      // Arrange
      var orderItems = new List<OrderItem>
            {
                new OrderItem { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 50m }
            };

      // Act
      await _repository.AddOrderItemsAsync(orderItems);
      await _repository.SaveChangesAsync();

      // Assert
      var savedItem = await _context.OrderItems.FindAsync(orderItems[0].Id);
      Assert.NotNull(savedItem);
      Assert.Equal(2, savedItem!.Quantity);
    }

    [Fact]
    public async Task GetAllAsync_Success_ReturnsOrders()
    {
      // Arrange
      var orders = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,
                    TotalAmount = 100m,
                    OrderCode = "ORD-123",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,
                    TotalAmount = 200m,
                    OrderCode = "ORD-124",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                }
            };
      _context.Orders.AddRange(orders);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetAllAsync(1, 10);

      // Assert
      Assert.Equal(2, result.Count());
      Assert.Contains(result, o => o.TotalAmount == 100m);
    }

    [Fact]
    public async Task CountAllAsync_Success_ReturnsCount()
    {
      // Arrange
      _context.Orders.AddRange(new[]
      {
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,
                    OrderCode = "ORD-123",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,
                    OrderCode = "ORD-124",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                }
            });
      await _context.SaveChangesAsync();

      // Act
      var count = await _repository.CountAllAsync();

      // Assert
      Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetByCustomerIdAsync_Success_ReturnsOrders()
    {
      // Arrange
      var customerId = _userId;
      var orders = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = customerId,
                    TotalAmount = 100m,
                    OrderCode = "ORD-123",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                }
            };
      _context.Orders.AddRange(orders);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetByCustomerIdAsync(customerId, 1, 10);

      // Assert
      Assert.Single(result);
      Assert.Equal(100m, result.First().TotalAmount);
    }

    [Fact]
    public async Task CountByCustomerIdAsync_Success_ReturnsCount()
    {
      // Arrange
      var customerId = _userId;
      _context.Orders.AddRange(new[]
      {
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = customerId,
                    OrderCode = "ORD-123",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = customerId,
                    OrderCode = "ORD-124",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                }
            });
      await _context.SaveChangesAsync();

      // Act
      var count = await _repository.CountByCustomerIdAsync(customerId);

      // Assert
      Assert.Equal(2, count);
    }

    [Fact]
    public async Task DeleteByUserIdAsync_Success_ReturnsTrue()
    {
      // Arrange
      var orders = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,
                    OrderCode = "ORD-123",
                    PaymentMethod = "CreditCard",
                    ShippingAddress = "123 Street",
                    Note = "",
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                }
            };
      _context.Orders.AddRange(orders);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.DeleteByUserIdAsync(_userId);

      // Assert
      Assert.True(result);
      Assert.Empty(await _context.Orders.Where(o => o.UserId == _userId).ToListAsync());
    }

    [Fact]
    public async Task DeleteByUserIdAsync_NoOrders_ReturnsFalse()
    {
      // Arrange
      var userId = _userId;

      // Act
      var result = await _repository.DeleteByUserIdAsync(userId);

      // Assert
      Assert.False(result);
    }
  }
}
