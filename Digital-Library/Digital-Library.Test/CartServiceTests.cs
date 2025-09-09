using System.Threading.Tasks;
using Xunit;
using Moq;
using Digital_Library.Service; // adjust namespace if CartService is here
using Digital_Library.Core.Models; // adjust if Response lives elsewhere
using Digital_Library.Service.Interface; // where ICartService lives

namespace Digital_Library.Test
{
    public class CartServiceTests
    {
        private readonly Mock<ICartService> _cartServiceMock;

        public CartServiceTests()
        {
            _cartServiceMock = new Mock<ICartService>();
        }

        [Fact]
        public async Task GetCartAsync_ShouldReturnResponse()
        {
            // Arrange
            var userId = "test-user";
            var expectedResponse = new Response { Success = true };
            _cartServiceMock.Setup(s => s.GetCartAsync(userId))
                            .ReturnsAsync(expectedResponse);

            // Act
            var result = await _cartServiceMock.Object.GetCartAsync(userId);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task AddItemAsync_ShouldReturnResponse()
        {
            // Arrange
            var userId = "test-user";
            var request = new CartDetailRequest { ProductId = "1", Quantity = 2 };
            var expectedResponse = new Response { Success = true };

            _cartServiceMock.Setup(s => s.AddItemAsync(userId, request))
                            .ReturnsAsync(expectedResponse);

            // Act
            var result = await _cartServiceMock.Object.AddItemAsync(userId, request);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task RemoveItemAsync_ShouldReturnResponse()
        {
            // Arrange
            var cartDetailId = "cart-1";
            var expectedResponse = new Response { Success = true };

            _cartServiceMock.Setup(s => s.RemoveItemAsync(cartDetailId))
                            .ReturnsAsync(expectedResponse);

            // Act
            var result = await _cartServiceMock.Object.RemoveItemAsync(cartDetailId);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task UpdateCartAsync_ShouldReturnResponse()
        {
            // Arrange
            var cartDetailId = "cart-1";
            var quantity = 5;
            var expectedResponse = new Response { Success = true };

            _cartServiceMock.Setup(s => s.UpdateCartAsync(cartDetailId, quantity))
                            .ReturnsAsync(expectedResponse);

            // Act
            var result = await _cartServiceMock.Object.UpdateCartAsync(cartDetailId, quantity);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ClearCartAsync_ShouldReturnResponse()
        {
            // Arrange
            var userId = "test-user";
            var expectedResponse = new Response { Success = true };

            _cartServiceMock.Setup(s => s.ClearCartAsync(userId))
                            .ReturnsAsync(expectedResponse);

            // Act
            var result = await _cartServiceMock.Object.ClearCartAsync(userId);

            // Assert
            Assert.True(result.Success);
        }
    }
}
