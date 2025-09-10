using Moq;
using Xunit;
using Digital_Library.Service.Interface;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Core.Models;
using Assert = Xunit.Assert; // adjust namespace for Category, CategoryRequest, Response

namespace Digital_Library.Tests
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;

        public CategoryServiceTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
        }

        [Fact]
        public async Task AddCategory_ShouldReturnSuccessResponse()
        {
            // Arrange
            var request = new CategoryRequest { CategoryName = "History" };
            var expectedResponse = new Response { Success = true, Message = "Category added" };

            _mockCategoryService
                .Setup(s => s.AddCategory(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCategoryService.Object.AddCategory(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Category added", result.Message);
        }

        [Fact]
        public async Task GetAllCategories_ShouldReturnListOfCategories()
        {
            // Arrange
            var expectedCategories = new List<Category>
            {
                new Category { Id = "1", CategoryName = "Science" },
                new Category { Id = "2", CategoryName = "Literature" }
            };

            _mockCategoryService
                .Setup(s => s.GetAllCategories())
                .ReturnsAsync(expectedCategories);

            // Act
            var result = await _mockCategoryService.Object.GetAllCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Collection(result,
                item => Assert.Equal("Science", item.CategoryName),
                item => Assert.Equal("Literature", item.CategoryName));
        }
    }
}
