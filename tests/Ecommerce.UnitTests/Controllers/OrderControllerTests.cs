using AutoMapper;
using Ecommerce.API.Controllers;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ecommerce.UnitTests.Controllers
{
  public class OrderControllerTests
  {
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly OrderController _orderController;
    private readonly AdminOrderController _adminOrderController;
    private readonly string _userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";

    public OrderControllerTests()
    {
      _mockOrderService = new Mock<IOrderService>();
      _mockMapper = new Mock<IMapper>();
      _orderController = new OrderController(_mockOrderService.Object, _mockMapper.Object);
      _adminOrderController = new AdminOrderController(_mockOrderService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateOrder_Success_ReturnsOk()
    {
      // Arrange
      var orderDto = new CreateOrderDto
      {
        Items = new List<OrderItemDto> { new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 1 } },
        ShippingAddress = "123 Street",
        PaymentMethod = "CreditCard"
      };
      var orderId = Guid.NewGuid();
      var result = Result.Success(orderId, "Order created successfully.");
      _mockOrderService.Setup(s => s.CreateOrderAsync(_userId, orderDto)).ReturnsAsync(result);

      // Act
      var actionResult = await _orderController.CreateOrder(orderDto);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<Guid>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(orderId, apiResponse.Data);
      Assert.Equal("Order created successfully.", apiResponse.Message);
    }

    [Fact]
    public async Task CreateOrder_Failure_ReturnsBadRequest()
    {
      // Arrange
      var orderDto = new CreateOrderDto
      {
        Items = new List<OrderItemDto> { new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 1 } }
      };
      var result = Result.Failure<Guid>("Product not found.");
      _mockOrderService.Setup(s => s.CreateOrderAsync(_userId, orderDto)).ReturnsAsync(result);

      // Act
      var actionResult = await _orderController.CreateOrder(orderDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<Guid>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Equal("Product not found.", apiResponse.Message);
    }

    [Fact]
    public async Task GetOrderDetail_Success_ReturnsOk()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var orderDto = new OrderDto { Id = orderId, TotalAmount = 100m };
      var result = Result.Success(orderDto, "Order details retrieved successfully.");
      _mockOrderService.Setup(s => s.GetOrderDetailsAsync(orderId)).ReturnsAsync(result);

      // Act
      var actionResult = await _orderController.GetOrderDetail(orderId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<OrderDto>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(orderDto, apiResponse.Data);
      Assert.Equal("Order details retrieved successfully.", apiResponse.Message);
    }

    [Fact]
    public async Task GetOrderDetail_Failure_ReturnsNotFound()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var result = Result.Failure<OrderDto>("Order not found.");
      _mockOrderService.Setup(s => s.GetOrderDetailsAsync(orderId)).ReturnsAsync(result);

      // Act
      var actionResult = await _orderController.GetOrderDetail(orderId);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Equal("Order not found.", apiResponse.Message);
    }

    [Fact]
    public async Task GetMyOrders_Success_ReturnsOk()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var pagedResult = new PagedResult<OrderDto>
      {
        Items = new List<OrderDto> { new OrderDto { Id = Guid.NewGuid(), TotalAmount = 100m } },
        TotalCount = 1,
        PageIndex = pageIndex,
        PageSize = pageSize
      };
      var result = Result.Success(pagedResult, "Orders retrieved successfully.");
      _mockOrderService.Setup(s => s.GetUserOrdersAsync(_userId, pageIndex, pageSize)).ReturnsAsync(result);

      // Act
      var actionResult = await _orderController.GetMyOrders(pageIndex, pageSize);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<PagedResult<OrderDto>>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(pagedResult, apiResponse.Data);
      Assert.Equal("Orders retrieved successfully.", apiResponse.Message);
    }

    [Fact]
    public async Task GetMyOrders_Failure_ReturnsBadRequest()
    {
      // Arrange
      var pageIndex = 1;
      var pageSize = 10;
      var result = Result.Failure<PagedResult<OrderDto>>("Failed to retrieve orders.");
      _mockOrderService.Setup(s => s.GetUserOrdersAsync(_userId, pageIndex, pageSize)).ReturnsAsync(result);

      // Act
      var actionResult = await _orderController.GetMyOrders(pageIndex, pageSize);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Equal("Failed to retrieve orders.", apiResponse.Message);
    }

    [Fact]
    public async Task CancelOrder_Success_ReturnsOk()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var result = Result.Success("Order cancelled and stock restored.");
      _mockOrderService.Setup(s => s.CancelOrderAsync(orderId)).ReturnsAsync(result);

      // Act
      var actionResult = await _orderController.CancelOrder(orderId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Order cancelled and stock restored.", apiResponse.Message);
    }

    [Fact]
    public async Task CancelOrder_Failure_ReturnsBadRequest()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var result = Result.Failure("Order not found.");
      _mockOrderService.Setup(s => s.CancelOrderAsync(orderId)).ReturnsAsync(result);

      // Act
      var actionResult = await _orderController.CancelOrder(orderId);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Equal("Order not found.", apiResponse.Message);
    }

    [Fact]
    public async Task GetOrderCode_Success_ReturnsOk()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var orderCode = "ORD-123456";
      var result = Result.Success(orderCode, "Order code retrieved successfully.");
      _mockOrderService.Setup(s => s.GetOrderCodeAsync(orderId)).ReturnsAsync(result);

      // Act
      var actionResult = await _orderController.GetOrderCode(orderId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(orderCode, apiResponse.Data);
      Assert.Equal("Order code retrieved successfully.", apiResponse.Message);
    }

    [Fact]
    public async Task GetOrderCode_Failure_ReturnsNotFound()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var result = Result.Failure<string>("Order not found.");
      _mockOrderService.Setup(s => s.GetOrderCodeAsync(orderId)).ReturnsAsync(result);

      // Act
      var actionResult = await _orderController.GetOrderCode(orderId);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Equal("Order not found.", apiResponse.Message);
    }

    [Fact]
    public async Task Admin_GetAllOrdersByCustomerId_Success_ReturnsOk()
    {
      // Arrange
      var customerId = _userId;
      var pageIndex = 1;
      var pageSize = 10;
      var pagedResult = new PagedResult<OrderDto>
      {
        Items = new List<OrderDto> { new OrderDto { Id = Guid.NewGuid(), TotalAmount = 100m } },
        TotalCount = 1,
        PageIndex = pageIndex,
        PageSize = pageSize
      };
      var result = Result.Success(pagedResult, "Orders retrieved successfully.");
      _mockOrderService.Setup(s => s.GetOrdersByCustomerIdAsync(customerId, pageIndex, pageSize)).ReturnsAsync(result);

      // Act
      var actionResult = await _adminOrderController.GetAllOrdersByCustomerId(customerId, pageIndex, pageSize);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<PagedResult<OrderDto>>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal(pagedResult, apiResponse.Data);
      Assert.Equal("Orders retrieved successfully.", apiResponse.Message);
    }

    [Fact]
    public async Task Admin_GetAllOrdersByCustomerId_Failure_ReturnsBadRequest()
    {
      // Arrange
      var customerId = _userId;
      var pageIndex = 1;
      var pageSize = 10;
      var result = Result.Failure<PagedResult<OrderDto>>("Failed to retrieve orders.");
      _mockOrderService.Setup(s => s.GetOrdersByCustomerIdAsync(customerId, pageIndex, pageSize)).ReturnsAsync(result);

      // Act
      var actionResult = await _adminOrderController.GetAllOrdersByCustomerId(customerId, pageIndex, pageSize);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Equal("Failed to retrieve orders.", apiResponse.Message);
    }

    [Fact]
    public async Task Admin_UpdateOrderStatus_Success_ReturnsOk()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var updateDto = new UpdateOrderDto { Status = "Shipped" };
      var result = Result.Success("Success");
      _mockOrderService.Setup(s => s.UpdateOrderAsync(orderId, updateDto)).ReturnsAsync(result);

      // Act
      var actionResult = await _adminOrderController.UpdateOrderStatus(orderId, updateDto);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Success", apiResponse.Message);
    }

    [Fact]
    public async Task Admin_UpdateOrderStatus_Failure_ReturnsBadRequest()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var updateDto = new UpdateOrderDto { Status = "InvalidStatus" };
      var result = Result.Failure("Invalid status.");
      _mockOrderService.Setup(s => s.UpdateOrderAsync(orderId, updateDto)).ReturnsAsync(result);

      // Act
      var actionResult = await _adminOrderController.UpdateOrderStatus(orderId, updateDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Equal("Invalid status.", apiResponse.Message);
    }

    [Fact]
    public async Task Admin_CancelOrder_Success_ReturnsOk()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var result = Result.Success("Order cancelled and stock restored.");
      _mockOrderService.Setup(s => s.CancelOrderAsync(orderId)).ReturnsAsync(result);

      // Act
      var actionResult = await _adminOrderController.CancelOrder(orderId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
      Assert.True(apiResponse.Status);
      Assert.Equal("Order cancelled and stock restored.", apiResponse.Message);
    }

    [Fact]
    public async Task Admin_CancelOrder_Failure_ReturnsBadRequest()
    {
      // Arrange
      var orderId = Guid.NewGuid();
      var result = Result.Failure("Order not found.");
      _mockOrderService.Setup(s => s.CancelOrderAsync(orderId)).ReturnsAsync(result);

      // Act
      var actionResult = await _adminOrderController.CancelOrder(orderId);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
      var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
      Assert.False(apiResponse.Status);
      Assert.Equal("Order not found.", apiResponse.Message);
    }
  }
}
