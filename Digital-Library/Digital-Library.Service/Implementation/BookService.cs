using Digital_Library.Core.Constant;
using Digital_Library.Core.Filters;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Net;

namespace Digital_Library.Service.Implementation
{
	public class BookService : IBookService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IFileService _fileService;
		private readonly ILogger<BookService> _logger;

		public BookService(IUnitOfWork unitOfWork, IFileService fileService, ILogger<BookService> logger)
		{
			_unitOfWork = unitOfWork;
			_fileService = fileService;
			_logger = logger;
		}

		public async Task<Response> AddBook(BookRequest request, string vendorId)
		{
			try
			{
				var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryID);
				if (category == null)
					return Response.Fail("Invalid Category ID.");

				string? pdfPath = null;
				if (request.PDFFile != null)
				{
					pdfPath = await _fileService.AddFile(request.PDFFile, FileFoldersName.BooksPdf);
				}

				string? coverPath = null;
				if (request.ImageBookCover != null)
				{
					coverPath = await _fileService.AddFile(request.ImageBookCover, FileFoldersName.BooksImageCover);
				}

				var book = new Book
				{
					Title = request.Title,
					Author = request.Author,
					PricePhysical = request.PricePhysical,
					PricePDFPerDay = request.PricePDFPerDay,
					Description = request.Description,
					Stock = request.Stock,
					HasPDF = request.HasPDF,
					IsBorrowable = request.IsBorrowable,
					CategoryID = request.CategoryID,
					VendorId = vendorId,
					PDFFilePath = pdfPath,
					ImageBookCoverPath = coverPath
				};

				await _unitOfWork.Books.AddAsync(book);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Book '{Title}' added successfully by Vendor {VendorId}", book.Title, vendorId);
				return Response.Ok("Book added successfully");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while adding book {Title} for Vendor {VendorId}", request.Title, vendorId);
				return Response.Fail("An error occurred while adding the book.");
			}
		}

		public async Task<Response> DeleteBook(string bookId)
		{
			try
			{
				var book = await _unitOfWork.Books.GetByIdAsync(bookId);
				if (book == null)
					return Response.Fail("Book not found.");

				if (!string.IsNullOrEmpty(book.PDFFilePath))
					await _fileService.DeleteFile(book.PDFFilePath);

				if (!string.IsNullOrEmpty(book.ImageBookCoverPath))
					await _fileService.DeleteFile(book.ImageBookCoverPath);

				_unitOfWork.Books.Delete(book);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Book '{BookId}' deleted successfully", bookId);
				return Response.Ok("Book deleted successfully");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while deleting book {BookId}", bookId);
				return Response.Fail("An error occurred while deleting the book.");
			}
		}

		public async Task<IEnumerable<Book>> GetAllBooks(BookFilter? filter = null)
		{
			_logger.LogInformation("Start GetAllBooks with filter: {@Filter}", filter);

			IQueryable<Book> query = _unitOfWork.Books.GetAllQuery(
							includes: new Expression<Func<Book, object>>[] { b => b.Category, b => b.Vendor }
			);

			if (filter != null)
			{
				if (!string.IsNullOrEmpty(filter.VendorId))
				{
					query = query.Where(b => b.VendorId == filter.VendorId);
					_logger.LogDebug("Applied VendorId filter: {VendorId}", filter.VendorId);
				}

				if (!string.IsNullOrEmpty(filter.CategoryId))
				{
					query = query.Where(b => b.CategoryID == filter.CategoryId);
					_logger.LogDebug("Applied CategoryId filter: {CategoryId}", filter.CategoryId);
				}

				if (filter.HasPDF.HasValue)
				{
					query = query.Where(b => b.HasPDF == filter.HasPDF.Value);
					_logger.LogDebug("Applied HasPDF filter: {HasPDF}", filter.HasPDF.Value);
				}

				if (filter.IsBorrowable.HasValue)
				{
					query = query.Where(b => b.IsBorrowable == filter.IsBorrowable.Value);
					_logger.LogDebug("Applied IsBorrowable filter: {IsBorrowable}", filter.IsBorrowable.Value);
				}

				if (!string.IsNullOrEmpty(filter.Keyword))
				{
					query = query.Where(b => b.Title.Contains(filter.Keyword) || b.Author.Contains(filter.Keyword));
					_logger.LogDebug("Applied Keyword filter: {Keyword}", filter.Keyword);
				}
			}

			var books = await query.ToListAsync();
			_logger.LogInformation("GetAllBooks retrieved {Count} records", books.Count);

			return books;
		}

		public async Task<Response> GetBookById(string bookId)
		{
			_logger.LogInformation("Start GetBookById with Id: {BookId}", bookId);

			var book = await _unitOfWork.Books.GetSingleAsync(
																			b => b.Id == bookId,
																			b => b.Category,
																			b => b.Vendor
			);

			if (book == null)
			{
				_logger.LogWarning("Book not found with Id: {BookId}", bookId);
				return Response.Fail("Book not found.");
			}

			_logger.LogInformation("Book '{BookId}' retrieved successfully", bookId);
			return Response.Ok("Book retrieved successfully", book);
		}

		public async Task<Response> UpdateBook(string bookId, BookRequest request)
		{
			try
			{
				var book = await _unitOfWork.Books.GetByIdAsync(bookId);
				if (book == null)
					return Response.Fail("Book not found.");

				book.Title = request.Title;
				book.Author = request.Author;
				book.PricePhysical = request.PricePhysical;
				book.PricePDFPerDay = request.PricePDFPerDay;
				book.Description = request.Description;
				book.Stock = request.Stock;
				book.HasPDF = request.HasPDF;
				book.IsBorrowable = request.IsBorrowable;
				book.CategoryID = request.CategoryID;

				if (request.PDFFile != null)
				{
					if (!string.IsNullOrEmpty(book.PDFFilePath))
						await _fileService.DeleteFile(book.PDFFilePath);

					book.PDFFilePath = await _fileService.AddFile(request.PDFFile, FileFoldersName.BooksPdf);
				}

				if (request.ImageBookCover != null)
				{
					if (!string.IsNullOrEmpty(book.ImageBookCoverPath))
						await _fileService.DeleteFile(book.ImageBookCoverPath);

					book.ImageBookCoverPath = await _fileService.AddFile(request.ImageBookCover, FileFoldersName.BooksImageCover);
				}

				_unitOfWork.Books.Update(book);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Book '{BookId}' updated successfully", bookId);
				return Response.Ok("Book updated successfully", book);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while updating book {BookId}", bookId);
				return Response.Fail("An error occurred while updating the book.");
			}
		}
	}
}
