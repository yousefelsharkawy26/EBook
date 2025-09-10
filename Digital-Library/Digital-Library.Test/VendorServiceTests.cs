//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Moq;
//using Xunit;
//using Digital_Library.Service.Interface;
//using Digital_Library.Models; // adjust namespaces for Response, Vendor, VendorRequest, VendorUpdateRequest, VendorStatus

//namespace Digital_Library.Tests
//{
//    public class VendorServiceTests
//    {
//        private readonly Mock<IVendorService> _mockVendorService;

//        public VendorServiceTests()
//        {
//            _mockVendorService = new Mock<IVendorService>();
//        }

//        [Fact]
//        public async Task SubmitVendorRequestAsync_ShouldReturnSuccessResponse()
//        {
//            // Arrange
//            var request = new VendorRequest { Name = "Book Store" };
//            var userId = "user123";
//            var expectedResponse = new Response { Success = true, Message = "Vendor request submitted" };

//            _mockVendorService
//                .Setup(s => s.SubmitVendorRequestAsync(request, userId))
//                .ReturnsAsync(expectedResponse);

//            // Act
//            var result = await _mockVendorService.Object.SubmitVendorRequestAsync(request, userId);

//            // Assert
//            Assert.True(result.Success);
//            Assert.Equal("Vendor request submitted", result.Message);
//        }

//        [Fact]
//        public async Task GetVendorByIdAsync_ShouldReturnVendor()
//        {
//            // Arrange
//            var vendorId = "vendor123";
//            var expectedVendor = new Vendor { Id = vendorId, Name = "Book Store" };

//            _mockVendorService
//                .Setup(s => s.GetVendorByIdAsync(vendorId, true))
//                .ReturnsAsync(expectedVendor);

//            // Act
//            var result = await _mockVendorService.Object.GetVendorByIdAsync(vendorId, true);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(vendorId, result.Id);
//        }

//        [Fact]
//        public async Task GetVendorsAsync_ShouldReturnVendorList()
//        {
//            // Arrange
//            var vendors = new List<Vendor>
//            {
//                new Vendor { Id = "1", Name = "Vendor A" },
//                new Vendor { Id = "2", Name = "Vendor B" }
//            };

//            _mockVendorService
//                .Setup(s => s.GetVendorsAsync(null))
//                .ReturnsAsync(vendors);

//            // Act
//            var result = await _mockVendorService.Object.GetVendorsAsync();

//            // Assert
//            Assert.Equal(2, ((List<Vendor>)result).Count);
//        }

//        [Fact]
//        public async Task UpdateVendorProfileAsync_ShouldReturnSuccessResponse()
//        {
//            // Arrange
//            var vendorId = "vendor123";
//            var request = new VendorUpdateRequest { Name = "Updated Vendor" };
//            var expectedResponse = new Response { Success = true, Message = "Vendor profile updated" };

//            _mockVendorService
//                .Setup(s => s.UpdateVendorProfileAsync(vendorId, request))
//                .ReturnsAsync(expectedResponse);

//            // Act
//            var result = await _mockVendorService.Object.UpdateVendorProfileAsync(vendorId, request);

//            // Assert
//            Assert.True(result.Success);
//            Assert.Equal("Vendor profile updated", result.Message);
//        }

//        [Fact]
//        public async Task ChangeStatusAsync_ShouldReturnSuccessResponse()
//        {
//            // Arrange
//            var vendorId = "vendor123";
//            var status = VendorStatus.Approved;
//            var expectedResponse = new Response { Success = true, Message = "Status changed" };

//            _mockVendorService
//                .Setup(s => s.ChangeStatusAsync(vendorId, status, null))
//                .ReturnsAsync(expectedResponse);

//            // Act
//            var result = await _mockVendorService.Object.ChangeStatusAsync(vendorId, status);

//            // Assert
//            Assert.True(result.Success);
//            Assert.Equal("Status changed", result.Message);
//        }

//        [Fact]
//        public async Task ReturnVendorIdFromUserId_ShouldReturnSuccessResponse()
//        {
//            // Arrange
//            var userId = "user123";
//            var expectedResponse = new Response { Success = true, Message = "VendorId found", Data = "vendor123" };

//            _mockVendorService
//                .Setup(s => s.ReturnVendorIdFromUserId(userId))
//                .ReturnsAsync(expectedResponse);

//            // Act
//            var result = await _mockVendorService.Object.ReturnVendorIdFromUserId(userId);

//            // Assert
//            Assert.True(result.Success);
//            Assert.Equal("VendorId found", result.Message);
//            Assert.Equal("vendor123", result.Data);
//        }
//    }
//}
