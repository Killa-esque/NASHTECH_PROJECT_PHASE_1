using AutoMapper;
using Ecommerce.Shared.DTOs;
using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Common;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Services;

public class OrderService : IOrderService
{
  private readonly IOrderRepository _orderRepository;
  private readonly IProductRepository _productRepository;
  private readonly ICartRepository _cartRepository;
  private readonly IMapper _mapper;

  public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper, ICartRepository cartRepository)
  {
    _cartRepository = cartRepository;
    _orderRepository = orderRepository;
    _productRepository = productRepository;
    _mapper = mapper;
  }

  public async Task<Result<PagedResult<OrderDto>>> GetAllOrdersAsync(int pageIndex, int pageSize)
  {
    var orders = await _orderRepository.GetAllAsync(pageIndex, pageSize);
    var totalCount = await _orderRepository.CountAllAsync();

    var orderDtos = _mapper.Map<List<OrderDto>>(orders);
    var pagedResult = PagedResult<OrderDto>.Create(orderDtos, totalCount, pageIndex, pageSize);

    return Result.Success(pagedResult, "Orders retrieved successfully.");
  }

  public async Task<Result<PagedResult<OrderDto>>> GetOrdersByCustomerIdAsync(string customerId, int pageIndex, int pageSize)
  {
    var orders = await _orderRepository.GetByCustomerIdAsync(customerId, pageIndex, pageSize);
    var totalCount = await _orderRepository.CountByCustomerIdAsync(customerId);

    var orderDtos = _mapper.Map<List<OrderDto>>(orders);
    var pagedResult = PagedResult<OrderDto>.Create(orderDtos, totalCount, pageIndex, pageSize);

    return Result.Success(pagedResult, "Orders retrieved successfully.");
  }

  public async Task<Result> CancelOrderAsync(Guid orderId)
  {
    var order = await _orderRepository.GetByIdAsync(orderId);
    if (order == null)
      return Result.Failure("Order not found.");

    if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Completed)
      return Result.Failure("Cannot cancel a shipped or completed order.");

    // 1. Lấy OrderItems để hoàn stock
    var orderItems = await _orderRepository.GetOrderItemsByOrderIdAsync(orderId);
    foreach (var item in orderItems)
    {
      var product = await _productRepository.GetByIdAsync(item.ProductId);
      product.Stock += item.Quantity; // Trả lại stock
      await _productRepository.UpdateAsync(product);
    }

    // 2. Huỷ đơn
    order.Status = OrderStatus.Cancelled;
    await _orderRepository.UpdateAsync(order);
    await _orderRepository.SaveChangesAsync();

    return Result.Success("Order cancelled and stock restored.");
  }

  public async Task<Result<Guid>> CreateOrderAsync(string userId, CreateOrderDto orderDto)
  {
    decimal totalAmount = 0;
    var orderItems = new List<OrderItem>();

    foreach (var itemDto in orderDto.Items)
    {
      var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
      if (product == null)
        return Result.Failure<Guid>($"Product with ID {itemDto.ProductId} not found.");

      // Kiểm tra tồn kho
      if (product.Stock < itemDto.Quantity)
        return Result.Failure<Guid>($"Product {product.Name} does not have enough stock.");

      var orderItem = new OrderItem
      {
        Id = Guid.NewGuid(),
        ProductId = product.Id,
        Quantity = itemDto.Quantity,
        UnitPrice = product.Price,
        OrderId = Guid.Empty // Set sau
      };

      totalAmount += product.Price * itemDto.Quantity;
      orderItems.Add(orderItem);
    }

    var order = new Order
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      OrderCode = $"ORD-{DateTime.UtcNow.Ticks}",
      ShippingAddress = orderDto.ShippingAddress,
      PaymentMethod = orderDto.PaymentMethod,
      Note = orderDto.Note,
      TotalAmount = totalAmount,
      Status = OrderStatus.Pending,
      CreatedDate = DateTime.UtcNow
    };

    foreach (var item in orderItems)
    {
      item.OrderId = order.Id;

      // Trừ stock
      var product = await _productRepository.GetByIdAsync(item.ProductId);
      product.Stock -= item.Quantity;
      await _productRepository.UpdateAsync(product);
    }


    await _orderRepository.AddAsync(order);
    await _orderRepository.AddOrderItemsAsync(orderItems);
    await _orderRepository.SaveChangesAsync();

    // Clear User Cart After Order Created
    var afftectedRows = await _cartRepository.ClearCartAsync(userId);

    if (afftectedRows == 0)
      return Result.Failure<Guid>("Failed to clear cart after order creation.");

    return Result.Success(order.Id, "Order created successfully.");
  }

  public async Task<Result<OrderDto>> GetOrderDetailsAsync(Guid orderId)
  {
    if (orderId == Guid.Empty)
      return Result.Failure<OrderDto>("ID đơn hàng không hợp lệ.");

    var order = await _orderRepository.GetByIdAsync(orderId);
    if (order == null)
      return Result.Failure<OrderDto>("Order not found.");

    var orderItems = await _orderRepository.GetOrderItemsByOrderIdAsync(orderId);

    // Lấy ProductName từ Products
    var productIds = orderItems.Select(oi => oi.ProductId).ToList();
    var products = await _productRepository.GetByIdsAsync(productIds);
    var productDict = products.ToDictionary(p => p.Id, p => p.Name);

    var orderDto = _mapper.Map<OrderDto>(order);
    orderDto.Items = orderItems.Select(oi => new OrderItemDto
    {
      Id = oi.Id,
      ProductId = oi.ProductId,
      ProductName = productDict.ContainsKey(oi.ProductId) ? productDict[oi.ProductId] : "Unknown",
      Quantity = oi.Quantity,
      Price = oi.UnitPrice
    }).ToList();

    return Result.Success(orderDto, "Order details retrieved successfully.");
  }

  public async Task<Result<string>> GetOrderStatusAsync(Guid orderId)
  {
    var order = await _orderRepository.GetByIdAsync(orderId);
    if (order == null)
      return Result.Failure<string>("Order not found.");

    return Result.Success(order.Status.ToString(), "Order status retrieved successfully.");
  }

  public async Task<Result<PagedResult<OrderDto>>> GetUserOrdersAsync(string userId, int pageIndex, int pageSize)
  {
    var orders = await _orderRepository.GetByUserIdAsync(userId, pageIndex, pageSize);
    var totalCount = await _orderRepository.CountByUserIdAsync(userId);

    var orderDtos = _mapper.Map<List<OrderDto>>(orders);
    var pagedResult = PagedResult<OrderDto>.Create(orderDtos, totalCount, pageIndex, pageSize);

    return Result.Success(pagedResult, "Orders retrieved successfully.");
  }

  public async Task<Result> UpdateOrderAsync(Guid orderId, UpdateOrderDto orderUpdateDto)
  {
    var order = await _orderRepository.GetByIdAsync(orderId);
    if (order == null)
      return Result.Failure("Order not found.");

    if (!CanModifyOrder(order))
      return Result.Failure("Cannot update a shipped or completed order.");

    if (!string.IsNullOrWhiteSpace(orderUpdateDto.Status))
    {
      if (!Enum.TryParse<OrderStatus>(orderUpdateDto.Status, true, out var parsedStatus))
        return Result.Failure("Invalid status.");

      order.Status = parsedStatus;
    }

    if (!string.IsNullOrWhiteSpace(orderUpdateDto.ShippingAddress))
      order.ShippingAddress = orderUpdateDto.ShippingAddress;

    if (!string.IsNullOrWhiteSpace(orderUpdateDto.Note))
      order.Note = orderUpdateDto.Note;

    order.UpdatedDate = DateTime.UtcNow;

    await _orderRepository.UpdateAsync(order);
    await _orderRepository.SaveChangesAsync();

    return Result.Success("Order updated successfully.");
  }

  public async Task<Result<string>> GetOrderCodeAsync(Guid orderId)
  {
    var order = await _orderRepository.GetByIdAsync(orderId);
    if (order == null)
      return Result.Failure<string>("Order not found.");

    return Result.Success(order.OrderCode, "Order code retrieved successfully.");
  }

  public async Task<Result<bool>> DeleteOrdersByUserIdAsync(string userId)
  {
    var orders = await _orderRepository.GetByUserIdAsync(userId, 1, int.MaxValue);
    if (orders == null || !orders.Any())
      return Result.Success(true, "No orders found for this user.");

    foreach (var order in orders)
    {
      if (CanModifyOrder(order))
      {
        order.Status = OrderStatus.Cancelled;
        await _orderRepository.UpdateAsync(order);
      }
    }

    await _orderRepository.SaveChangesAsync();

    // After canceling orders, delete them
    var result = await _orderRepository.DeleteByUserIdAsync(userId);

    if (result == false)
    {
      return Result.Success(true, "User not found or no orders to delete.");
    }

    await _orderRepository.SaveChangesAsync();

    return Result.Success(true, "Orders deleted successfully.");
  }

  private bool CanModifyOrder(Order order)
  {
    return order.Status != OrderStatus.Shipped && order.Status != OrderStatus.Completed;
  }
}

