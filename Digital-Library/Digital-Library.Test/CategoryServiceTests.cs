using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Digital_Library.Service.Interface;
using Digital_Library.Models; // adjust namespace for Category, CategoryRequest, Response

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
            var request = new CategoryRequest { Name = "History" };
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
                new Category { Id = "1", Name = "Science" },
                new Category { Id = "2", Name = "Literature" }
            };

            _mockCategoryService
                .Setup(s => s.GetAllCategories())
                .ReturnsAsync(expectedCategories);

            // Act
            var result = await _mockCategoryService.Object.GetAllCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Collection(result,
                item => Assert.Equal("Science", item.Name),
                item => Assert.Equal("Literature", item.Name));
        }
    }
}
