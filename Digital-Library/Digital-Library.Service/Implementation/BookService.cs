using Azure;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Infrastructure.Repositories.Interface;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Digital_Library.Service.Implementation
{
	public class BookService : IBookService
	{

		private readonly IUnitOfWork _unitOfWork;

		public BookService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public Task<Response> AddBook(BookRequest request, string vendorId)
		{
			throw new NotImplementedException();
		}

		public Task<Response> DeleteBook(string bookId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Book>> GetAllBooks(string? vendorId)
		{
			throw new NotImplementedException();
		}

		public Task<Book> GetBookById(string book)
		{
			throw new NotImplementedException();
		}

		public Task<Response> UpdateBook(string bookId, BookRequest request)
		{
			throw new NotImplementedException();
		}
	}
}
