using Digital_Library.Core.Enums;
using Digital_Library.Core.Filters;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;

namespace Digital_Library.Service.Interface
{
	public interface IBookService
	{
		Task<Response> AddBook(BookRequest request, string vendorId);

		Task<Response> UpdateBook(string bookId, UpdateBookRequest request, string newPathCover = null);

		Task<Response> DeleteBook(string bookId);

		Task<Response> GetBookById(string bookId);

		IQueryable<Book> GetAllBooks(BookFilter? filter = null);

		Task<IEnumerable<Book>> GetBestTenSellingBook();

		Task<(IEnumerable<Book> Books, int TotalCount)> GetPagedBooksAsync(string Vid, int page, int pageSize, BookFilter? filter = null);

		Task<IEnumerable<Book>> GetRelatedBooksAsync(string excludeBookId, int count = 3);
		Task<PagedResult<UserBookDto>> GetUserBooksAsync(string userId, int pageNumber, int pageSize);

		Task<IEnumerable<BookSummaryViewModel>> GetAllBooksAsync();
		Task<BookFormViewModel?> GetBookForEditAsync(string bookId);
		Task<BookFormViewModel> GetBookCreationDataAsync(); 
		Task CreateBookAsync(BookFormViewModel model);
		Task UpdateBookAsync(BookFormViewModel model);
	}
}
