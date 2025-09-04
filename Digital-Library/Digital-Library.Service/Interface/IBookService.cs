using Digital_Library.Core.Filters;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;

namespace Digital_Library.Service.Interface
{
	public interface IBookService
	{
		Task<Response> AddBook(BookRequest request, string vendorId);

		Task<Response> UpdateBook(string bookId, BookRequest request);

		Task<Response> DeleteBook(string bookId);

		Task<Response> GetBookById(string bookId);

		Task<IEnumerable<Book>> GetAllBooks(BookFilter? filter = null);

	}
}
