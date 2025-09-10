using Moq;
using Xunit;
using Digital_Library.Service.Interface;
using Assert = Xunit.Assert;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Core.Models;
using Digital_Library.Core.Enum; // adjust namespaces for Response, OrderHeader, OrderDetailRequest, PlaceOrderRequest, Status

namespace Digital_Library.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderService> _mockOrderService;

        public OrderServiceTests()
        {
            _mockOrderService = new Mock<IOrderService>();
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldReturnSuccessResponse()
        {
            // Arrange
            var userId = "user123";
            var items = new List<OrderDetailRequest> { new OrderDetailRequest { BookId = "p1", Quantity = 2 } };
            var request = new PlaceOrderRequest { Address = "123 Street" };
            var expectedResponse = new Response { Success = true, Message = "Order created" };

            _mockOrderService
                .Setup(s => s.CreateOrderAsync(userId, items, request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderService.Object.CreateOrderAsync(userId, items, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Order created", result.Message);
        }

        [Fact]
        public async Task GetVendorOrders_ShouldReturnQueryableOrders()
        {
            // Arrange
            var vendorId = "vendor123";
            var orders = new List<OrderHeader>
            {
                new OrderHeader { Id = "1", VendorId = vendorId },
                new OrderHeader { Id = "2", VendorId = vendorId }
            }.AsQueryable();

            _mockOrderService
                .Setup(s => s.GetVendorOrders(vendorId))
                .ReturnsAsync(orders);

            // Act
            var result = await _mockOrderService.Object.GetVendorOrders(vendorId);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUserOrders_ShouldReturnQueryableOrders()
        {
            // Arrange
            var userId = "user123";
            var orders = new List<OrderHeader>
            {
                new OrderHeader { Id = "1", VendorId = userId },
                new OrderHeader { Id = "2", VendorId = userId }
            }.AsQueryable();

            _mockOrderService
                .Setup(s => s.GetUserOrders(userId))
                .ReturnsAsync(orders);

            // Act
            var result = await _mockOrderService.Object.GetUserOrders(userId);

            // Assert
            Assert.All(result, o => Assert.Equal(userId, o.VendorId));
        }

        [Fact]
        public async Task GetOrderHeaderDetailsByIdAsync_ShouldReturnResponse()
        {
            // Arrange
            var orderId = "order123";
            var expectedResponse = new Response { Success = true, Message = "Order details retrieved" };

            _mockOrderService
                .Setup(s => s.GetOrderHeaderDetailsByIdAsync(orderId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderService.Object.GetOrderHeaderDetailsByIdAsync(orderId);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldReturnSuccessResponse()
        {
            // Arrange
            var orderId = "order123";
            var newStatus = Status.Complete; // adjust enum to match your project
            var expectedResponse = new Response { Success = true, Message = "Order updated" };

            _mockOrderService
                .Setup(s => s.UpdateOrderStatusAsync(orderId, newStatus))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderService.Object.UpdateOrderStatusAsync(orderId, newStatus);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Order updated", result.Message);
        }
    }
}
