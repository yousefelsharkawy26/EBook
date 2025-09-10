using System.Threading.Tasks;
using Moq;
using Xunit;
using Digital_Library.Service.Interface;
using Assert = Xunit.Assert;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Core.ViewModels.Requests;
namespace Digital_Library.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserService> _mockUserService;

        public UserServiceTests()
        {
            _mockUserService = new Mock<IUserService>();
        }

        [Fact]
        public async Task UpdateProfileAsync_ShouldReturnSuccessResponse()
        {
            // Arrange
            var userId = "user123";
            var request = new UserRequest { FullName = "John Doe", ImageProfile = null};
            var expectedResponse = new Response { Success = true, Message = "Profile updated" };

            _mockUserService
                .Setup(s => s.UpdateProfileAsync(userId, request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUserService.Object.UpdateProfileAsync(userId, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Profile updated", result.Message);
        }

        [Fact]
        public async Task GetProfileAsync_ShouldReturnUserProfile()
        {
            // Arrange
            var userId = "user123";
            var expectedResponse = new Response { Success = true, Message = "Profile retrieved" };

            _mockUserService
                .Setup(s => s.GetProfileAsync(userId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUserService.Object.GetProfileAsync(userId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Profile retrieved", result.Message);
        }
    }
}
