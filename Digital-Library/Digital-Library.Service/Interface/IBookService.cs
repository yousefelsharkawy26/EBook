using Azure;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Interface
{
	public interface IBookService
	{
		Task<Response> AddBook(BookRequest request, string vendorId);

		Task<Response> UpdateBook(string bookId, BookRequest request);

		Task<Response> DeleteBook(string bookId);

		Task<Book> GetBookById(string book);

		Task<IEnumerable<Book>> GetAllBooks(string? vendorId);








	}
}
