using Digital_Library.Core.Constant;
using Digital_Library.Core.Enums;
using Digital_Library.Core.Filters;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
					PricePdf = request.PricePDF,
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

		public async Task<Response> UpdateBook(string bookId, UpdateBookRequest request)
		{
			try
			{
				var book = await _unitOfWork.Books.GetByIdAsync(bookId);
				if (book == null)
					return Response.Fail("Book not found.");

				book.Title = request.Title;
				book.Author = request.Author;
				book.PricePhysical = request.PricePhysical;
				book.PricePdf = request.PricePDF;
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

		public async Task<IEnumerable<Book>> GetBestTenSellingBook()
		{
			var orders = _unitOfWork.OrderDetails.GetAllQuery(
				includes: new Expression<Func<OrderDetail, object>>[] { o => o.Book }
				);

			var ordersGroup = orders.GroupBy(oi => oi.Book )
									.Select(g => new BookGroupViewModel
									{
										Book = g.Key,
										TotalSold = g.Sum(x => x.Quantity),
									})
									.OrderByDescending(x => x.TotalSold)
									.Take(10);

            var bestFiveSales = ordersGroup.Select(o => o.Book);


            return await bestFiveSales.ToListAsync();
        }

  public async Task<(IEnumerable<Book> Books, int TotalCount)> GetPagedBooksAsync(
     string vendorId, int page, int pageSize, BookFilter? filter = null)
        {
            _logger.LogInformation(
                "Start GetPagedBooks with vendorId={VendorId}, page={Page}, pageSize={PageSize}, filter={@Filter}",
                vendorId, page, pageSize, filter);

            IQueryable<Book> query = _unitOfWork.Books.GetAllQuery(
                includes: new Expression<Func<Book, object>>[] { b => b.Category, b => b.Vendor }
            );

            
            query = query.Where(b => b.VendorId == vendorId);

            
            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.CategoryId))
                    query = query.Where(b => b.CategoryID == filter.CategoryId);

                if (filter.HasPDF.HasValue)
                    query = query.Where(b => b.HasPDF == filter.HasPDF.Value);

                if (filter.IsBorrowable.HasValue)
                    query = query.Where(b => b.IsBorrowable == filter.IsBorrowable.Value);

                if (!string.IsNullOrEmpty(filter.Keyword))
                    query = query.Where(b => b.Title.Contains(filter.Keyword) || b.Author.Contains(filter.Keyword));
            }

            int totalCount = await query.CountAsync();

            var books = await query
                .OrderBy(b => b.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("GetPagedBooks returned {Count} records out of {TotalCount}", books.Count, totalCount);

            return (books, totalCount);
        }

		public IQueryable<Book> GetAllBooks(BookFilter? filter = null)
		{
			IQueryable<Book> query = _unitOfWork.Books.GetAllQuery(
							includes: new Expression<Func<Book, object>>[] { b => b.Category, b => b.Vendor }
			);

			if (filter != null)
			{
				if (!string.IsNullOrEmpty(filter.VendorId))
					query = query.Where(b => b.VendorId == filter.VendorId);

				if (!string.IsNullOrEmpty(filter.CategoryId))
					query = query.Where(b => b.CategoryID == filter.CategoryId);

				if (filter.HasPDF.HasValue)
					query = query.Where(b => b.HasPDF == filter.HasPDF.Value);

				if (filter.IsBorrowable.HasValue)
					query = query.Where(b => b.IsBorrowable == filter.IsBorrowable.Value);

				if (!string.IsNullOrEmpty(filter.Keyword))
					query = query.Where(b => b.Title.Contains(filter.Keyword) || b.Author.Contains(filter.Keyword));
			}

			return query; 
		}

		public async Task<IEnumerable<Book>> GetRelatedBooksAsync(string categoryId, string excludeBookId, int count = 3)
		{
			return await _unitOfWork.Books.GetAllQuery()
							.Where(b => b.CategoryID == categoryId && b.Id != excludeBookId)
							.Take(count)
							.ToListAsync();
		}

		public async Task<List<UserBookDto>> GetUserBooksAsync(string userId)
		{
			var pdfBooks = _unitOfWork.UserPdfBooks
							.GetManyQuery(upb => upb.UserId == userId)
							.Select(upb => new UserBookDto
							{
								BookId = upb.Book.Id,
								Title = upb.Book.Title,
								Author = upb.Book.Author,
								Type = FormatType.PDF
							});
			var borrowedBooks = _unitOfWork.Borrowings
							.GetManyQuery(b => b.UserId == userId)
							.Select(b => new UserBookDto
							{
								BookId = b.Book.Id,
								Title = b.Book.Title,
								Author = b.Book.Author,
								Type = FormatType.Borrowing
							});
			var result = await pdfBooks
							.Union(borrowedBooks) 
							.ToListAsync();

			return result;
		}


	}
}

