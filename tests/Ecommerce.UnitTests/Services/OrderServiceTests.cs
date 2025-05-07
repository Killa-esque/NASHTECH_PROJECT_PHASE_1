using AutoMapper;
using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Application.Services;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Moq;

namespace Ecommerce.UnitTests.Services
{
  public class OrderServiceTests
  {
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly OrderService _service;
    private readonly string _userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";

    public OrderServiceTests()
    {
      _mockOrderRepository = new Mock<IOrderRepository>();
      _mockProductRepository = new Mock<IProductRepository>();
      _mockCartRepository = new Mock<ICartRepository>();
      _mockMapper = new Mock<IMapper>();
      _service = new OrderService(_mockOrderRepository.Object, _mockProductRepository.Object, _mockMapper.Object, _mockCartRepository.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_Success_ReturnsOrderId()
    {
      // Arrange
      var orderDto = new CreateOrderDto
      {
        Items = new List<OrderItemDto> { new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 2 } },
        ShippingAddress = "123 Street",
        PaymentMethod = "CreditCard"
      };
      var product = new Product { Id = orderDto.Items[0].ProductId, Name = "Product1", Price = 10m, Stock = 10 };
      var orderId = Guid.NewGuid();
      var order = new Order { Id = orderId, UserId = _userId, TotalAmount = 20m };
      var orderDtoMapped = new OrderDto { Id = orderId, TotalAmount = 20m };

      _mockProductRepository.Setup(r => r.GetByIdAsync(orderDto.Items[0].ProductId)).ReturnsAsync(product);
      _mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.FromResult(1));
      _mockOrderRepository.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
      _mockOrderRepository.Setup(r => r.AddOrderItemsAsync(It.IsAny<List<OrderItem>>())).Returns(Task.CompletedTask);
      _mockOrderRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
      _mockCartRepository.Setup(r => r.ClearCartAsync(_userId)).ReturnsAsync(1);
      _mockMapper.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(orderDtoMapped);

      // Act
      var result = await _service.CreateOrderAsync(_userId, orderDto);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Order created successfully.", result.Message);
      Assert.NotEqual(Guid.Empty, result.Data); // Kiểm tra Guid hợp lệ
      _mockProductRepository.Verify(r => r.UpdateAsync(It.Is<Product>(p => p.Stock == 8)), Times.Once());
    }

    [Fact]
    public async Task CreateOrderAsync_ProductNotFound_ReturnsFailure()
    {
      // Arrange
      var orderDto = new CreateOrderDto
      {
        Items = new List<OrderItemDto> { new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 1 } }
      };
      _mockProductRepository.Setup(r => r.GetByIdAsync(orderDto.Items[0].ProductId)).ReturnsAsync((Product?)null);

      // Act
      var result = await _service.CreateOrderAsync(_userId, orderDto);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Equal($"Product with ID {orderDto.Items[0].ProductId} not found.", result.Error);
    }

    [Fact]
    public async Task CreateOrderAsync_InsufficientStock_ReturnsFailure()
    {
      // Arrange
      var orderDto = new CreateOrderDto
      {
        Items = new List<OrderItemDto> { new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 5 } }
      };
      var product = new Product { Id = orderDto.Items[0].ProductId, Name = "Product1", Stock = 2 };
      _mockProductRepository.Setup(r => r.GetByIdAsync(orderDto.Items[0].ProductId)).ReturnsAsync(product);

      // Act
      var result = await _service.CreateOrderAsync(_userId, orderDto);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Equal($"Product {product.Name} does not have enough stock.", result.Error);
    }

    [Fact]
    public async Task GetOrderDetailsAsync_Success_ReturnsOrderDto()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var order = new Order { Id = orderId, UserId = _userId, TotalAmount = 100m };
      var orderItems = new List<OrderItem> { new OrderItem { Id = Guid.NewGuid(), OrderId = orderId, ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 50m } };
      var product = new Product { Id = orderItems[0].ProductId, Name = "Product1" };
      var orderDto = new OrderDto { Id = orderId, TotalAmount = 100m, Items = new List<OrderItemDto> { new OrderItemDto { ProductId = orderItems[0].ProductId, ProductName = "Product1", Quantity = 2, Price = 50m } } };

      _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
      _mockOrderRepository.Setup(r => r.GetOrderItemsByOrderIdAsync(orderId)).ReturnsAsync(orderItems);
      _mockProductRepository.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Product> { product });
      _mockMapper.Setup(m => m.Map<OrderDto>(order)).Returns(orderDto);

      // Act
      var result = await _service.GetOrderDetailsAsync(orderId);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Order details retrieved successfully.", result.Message);
      Assert.Equal(orderDto, result.Data);
      Assert.Single(result.Data!.Items);
      Assert.Equal("Product1", result.Data.Items[0].ProductName);
    }

    [Fact]
    public async Task GetOrderDetailsAsync_OrderNotFound_ReturnsFailure()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

      // Act
      var result = await _service.GetOrderDetailsAsync(orderId);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Equal("Order not found.", result.Error);
    }

    [Fact]
    public async Task GetUserOrdersAsync_Success_ReturnsPagedResult()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var orders = new List<Order> { new Order { Id = Guid.NewGuid(), UserId = _userId, TotalAmount = 100m } };
      var orderDtos = new List<OrderDto> { new OrderDto { Id = orders[0].Id, TotalAmount = 100m } };
      var pagedResult = PagedResult<OrderDto>.Create(orderDtos, 1, pageIndex, pageSize);

      _mockOrderRepository.Setup(r => r.GetByUserIdAsync(_userId, pageIndex, pageSize)).ReturnsAsync(orders);
      _mockOrderRepository.Setup(r => r.CountByUserIdAsync(_userId)).ReturnsAsync(1);
      _mockMapper.Setup(m => m.Map<List<OrderDto>>(orders)).Returns(orderDtos);

      // Act
      var result = await _service.GetUserOrdersAsync(_userId, pageIndex, pageSize);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Orders retrieved successfully.", result.Message);
      Assert.Equal(1, result.Data!.TotalCount);
      Assert.Single(result.Data.Items);
    }

    [Fact]
    public async Task CancelOrderAsync_Success_ReturnsSuccess()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var order = new Order { Id = orderId, UserId = _userId, Status = OrderStatus.Pending };
      var orderItems = new List<OrderItem> { new OrderItem { ProductId = Guid.NewGuid(), Quantity = 2 } };
      var product = new Product { Id = orderItems[0].ProductId, Stock = 8 };

      _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
      _mockOrderRepository.Setup(r => r.GetOrderItemsByOrderIdAsync(orderId)).ReturnsAsync(orderItems);
      _mockProductRepository.Setup(r => r.GetByIdAsync(orderItems[0].ProductId)).ReturnsAsync(product);
      _mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.FromResult(1));
      _mockOrderRepository.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
      _mockOrderRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

      // Act
      var result = await _service.CancelOrderAsync(orderId);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Order cancelled and stock restored.", result.Message);
      _mockProductRepository.Verify(r => r.UpdateAsync(It.Is<Product>(p => p.Stock == 10)), Times.Once());
    }

    [Fact]
    public async Task CancelOrderAsync_OrderNotFound_ReturnsFailure()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

      // Act
      var result = await _service.CancelOrderAsync(orderId);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Equal("Order not found.", result.Error);
    }

    [Fact]
    public async Task CancelOrderAsync_ShippedOrder_ReturnsFailure()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var order = new Order { Id = orderId, UserId = _userId, Status = OrderStatus.Shipped };
      _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

      // Act
      var result = await _service.CancelOrderAsync(orderId);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Equal("Cannot cancel a shipped or completed order.", result.Error);
    }

    [Fact]
    public async Task UpdateOrderAsync_Success_ReturnsSuccess()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var updateDto = new UpdateOrderDto { Status = "Shipped", ShippingAddress = "456 Street", Note = "Updated" };
      var order = new Order { Id = orderId, UserId = _userId, Status = OrderStatus.Pending };

      _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
      _mockOrderRepository.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
      _mockOrderRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

      // Act
      var result = await _service.UpdateOrderAsync(orderId, updateDto);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Order updated successfully.", result.Message);
      _mockOrderRepository.Verify(r => r.UpdateAsync(It.Is<Order>(o => o.Status == OrderStatus.Shipped && o.ShippingAddress == "456 Street" && o.Note == "Updated")), Times.Once());
    }

    [Fact]
    public async Task UpdateOrderAsync_InvalidStatus_ReturnsFailure()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var updateDto = new UpdateOrderDto { Status = "InvalidStatus" };
      var order = new Order { Id = orderId, UserId = _userId, Status = OrderStatus.Pending };

      _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

      // Act
      var result = await _service.UpdateOrderAsync(orderId, updateDto);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Equal("Invalid status.", result.Error);
    }

    [Fact]
    public async Task GetOrderCodeAsync_Success_ReturnsOrderCode()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var order = new Order { Id = orderId, OrderCode = "ORD-123456" };
      _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

      // Act
      var result = await _service.GetOrderCodeAsync(orderId);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Order code retrieved successfully.", result.Message);
      Assert.Equal("ORD-123456", result.Data);
    }

    [Fact]
    public async Task GetOrderCodeAsync_OrderNotFound_ReturnsFailure()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

      // Act
      var result = await _service.GetOrderCodeAsync(orderId);

      // Assert
      Assert.False(result.IsSuccess);
      Assert.Equal("Order not found.", result.Error);
    }

    [Fact]
    public async Task GetOrdersByCustomerIdAsync_Success_ReturnsPagedResult()
    {
      // Arrange
      var customerId = _userId;
      var pageIndex = 1;
      var pageSize = 10;
      var orders = new List<Order> { new Order { Id = Guid.NewGuid(), UserId = customerId, TotalAmount = 100m } };
      var orderDtos = new List<OrderDto> { new OrderDto { Id = orders[0].Id, TotalAmount = 100m } };
      var pagedResult = PagedResult<OrderDto>.Create(orderDtos, 1, pageIndex, pageSize);

      _mockOrderRepository.Setup(r => r.GetByCustomerIdAsync(customerId, pageIndex, pageSize)).ReturnsAsync(orders);
      _mockOrderRepository.Setup(r => r.CountByCustomerIdAsync(customerId)).ReturnsAsync(1);
      _mockMapper.Setup(m => m.Map<List<OrderDto>>(orders)).Returns(orderDtos);

      // Act
      var result = await _service.GetOrdersByCustomerIdAsync(customerId, pageIndex, pageSize);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Orders retrieved successfully.", result.Message);
      Assert.Equal(1, result.Data!.TotalCount);
      Assert.Single(result.Data.Items);
    }

    [Fact]
    public async Task DeleteOrdersByUserIdAsync_Success_ReturnsSuccess()
    {
      // Arrange
      var userId = _userId;
      var orders = new List<Order> { new Order { Id = Guid.NewGuid(), UserId = userId, Status = OrderStatus.Pending } };
      _mockOrderRepository.Setup(r => r.GetByUserIdAsync(userId, 1, int.MaxValue)).ReturnsAsync(orders);
      _mockOrderRepository.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
      _mockOrderRepository.Setup(r => r.DeleteByUserIdAsync(userId)).ReturnsAsync(true);
      _mockOrderRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

      // Act
      var result = await _service.DeleteOrdersByUserIdAsync(userId);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("Orders deleted successfully.", result.Message);
      Assert.True(result.Data);
    }

    [Fact]
    public async Task DeleteOrdersByUserIdAsync_NoOrders_ReturnsSuccess()
    {
      // Arrange
      var userId = _userId;
      _mockOrderRepository.Setup(r => r.GetByUserIdAsync(userId, 1, int.MaxValue)).ReturnsAsync(new List<Order>());
      _mockOrderRepository.Setup(r => r.DeleteByUserIdAsync(userId)).ReturnsAsync(false);

      // Act
      var result = await _service.DeleteOrdersByUserIdAsync(userId);

      // Assert
      Assert.True(result.IsSuccess);
      Assert.Equal("No orders found for this user.", result.Message);
      Assert.True(result.Data);
    }
  }
}
