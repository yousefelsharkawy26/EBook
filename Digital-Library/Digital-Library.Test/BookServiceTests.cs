// BookServiceTests.cs
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Infrastructure.Repositories.Interface;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Digital_Library.Test;
public class BookServiceTests
{
    // Mocks
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<ILogger<BookService>> _mockLogger;

    // Repositories Mocks (nested inside UnitOfWork)
    private readonly Mock<IBaseRepository<Book>> _mockBookRepo;
    private readonly Mock<IBaseRepository<Category>> _mockCategoryRepo;
    private readonly Mock<IBaseRepository<OrderDetail>> _mockOrderDetailRepo;

    // Service under test
    private readonly IBookService _bookService;

    public BookServiceTests()
    {
        // Setup top-level mocks
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockFileService = new Mock<IFileService>();
        _mockLogger = new Mock<ILogger<BookService>>();

        // Setup repository mocks
        _mockBookRepo = new Mock<IBaseRepository<Book>>();
        _mockCategoryRepo = new Mock<IBaseRepository<Category>>();
        _mockOrderDetailRepo = new Mock<IBaseRepository<OrderDetail>>();

        // Configure UnitOfWork mock to return repository mocks
        _mockUnitOfWork.Setup(uow => uow.Books).Returns(_mockBookRepo.Object);
        _mockUnitOfWork.Setup(uow => uow.Categories).Returns(_mockCategoryRepo.Object);
        _mockUnitOfWork.Setup(uow => uow.OrderDetails).Returns(_mockOrderDetailRepo.Object);

        // Instantiate the service with mocks
        _bookService = new BookService(
            _mockUnitOfWork.Object,
            _mockFileService.Object,
            _mockLogger.Object
        );
    }

    // Helper to create a mock IFormFile
    private IFormFile CreateMockFormFile()
    {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1); // Non-empty file
        return mockFile.Object;
    }

    // --- الاختبارات تبدأ من هنا ---
    [Fact]
    public async Task AddBook_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new BookRequest
        {
            Title = "New Book",
            Author = "Author Name",
            CategoryID = "cat1",
            PDFFile = CreateMockFormFile(),
            ImageBookCover = CreateMockFormFile()
        };
        var vendorId = "vendor1";
        var category = new Category { Id = "cat1", CategoryName = "Fiction" };

        _mockCategoryRepo.Setup(r => r.GetByIdAsync("cat1")).ReturnsAsync(category);
        _mockFileService.Setup(fs => fs.AddFile(request.PDFFile, It.IsAny<string>())).ReturnsAsync("path/to/book.pdf");
        _mockFileService.Setup(fs => fs.AddFile(request.ImageBookCover, It.IsAny<string>())).ReturnsAsync("path/to/cover.jpg");
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _bookService.AddBook(request, vendorId);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Book added successfully");
        _mockBookRepo.Verify(r => r.AddAsync(It.Is<Book>(b => b.Title == request.Title && b.VendorId == vendorId)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddBook_WithInvalidCategoryId_ShouldReturnFail()
    {
        // Arrange
        var request = new BookRequest { CategoryID = "invalid-cat" };
        _mockCategoryRepo.Setup(r => r.GetByIdAsync("invalid-cat")).ReturnsAsync((Category)null);

        // Act
        var result = await _bookService.AddBook(request, "vendor1");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid Category ID.");
        _mockBookRepo.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
    }

    [Fact]
    public async Task DeleteBook_WhenBookExists_ShouldReturnSuccess()
    {
        // Arrange
        var bookId = "book1";
        var book = new Book { Id = bookId, PDFFilePath = "path/to/book.pdf", ImageBookCoverPath = "path/to/cover.jpg" };

        _mockBookRepo.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _bookService.DeleteBook(bookId);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Book deleted successfully");
        _mockFileService.Verify(fs => fs.DeleteFile(book.PDFFilePath), Times.Once);
        _mockFileService.Verify(fs => fs.DeleteFile(book.ImageBookCoverPath), Times.Once);
        _mockBookRepo.Verify(r => r.Delete(book), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteBook_WhenBookDoesNotExist_ShouldReturnFail()
    {
        // Arrange
        var bookId = "non-existent-book";
        _mockBookRepo.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync((Book)null);

        // Act
        var result = await _bookService.DeleteBook(bookId);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Book not found.");
        _mockBookRepo.Verify(r => r.Delete(It.IsAny<Book>()), Times.Never);
    }

    
}
