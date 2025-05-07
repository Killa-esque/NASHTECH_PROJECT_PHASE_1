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
  public class AdminOrderControllerTests
  {
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AdminOrderController _adminOrderController;
    private readonly string _userId = "a8a9b907-81a6-41c9-90ff-eb2158c52044";

    public AdminOrderControllerTests()
    {
      _mockOrderService = new Mock<IOrderService>();
      _mockMapper = new Mock<IMapper>();
      _adminOrderController = new AdminOrderController(_mockOrderService.Object, _mockMapper.Object);
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
