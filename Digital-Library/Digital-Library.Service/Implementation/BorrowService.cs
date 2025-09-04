using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Implementation
{
	public class BorrowService : IBorrowService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<BorrowService> _logger;

		public BorrowService(IUnitOfWork unitOfWork, ILogger<BorrowService> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task<Response> BorrowBookAsync(string userId, BorrowRequest request)
		{
			try
			{
				var book = await _unitOfWork.Books.GetByIdAsync(request.BookId);
				if (book == null)
					return Response.Fail("Book not found.");

				if (!book.IsBorrowable)
					return Response.Fail("This book is not available for borrowing.");


				var borrowing = new Borrowing
				{
					BookId = request.BookId,
					UserId = userId,
					BorrowDate = DateTime.Now,
					DueDate = DateTime.Now.AddDays(request.Days)
				};

				await _unitOfWork.Borrowings.AddAsync(borrowing);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("User '{UserId}' borrowed book '{BookId}' for {Days} days", userId, request.BookId, request.Days);
				return Response.Ok("Book borrowed successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error borrowing book '{BookId}' for user '{UserId}'", request.BookId, userId);
				return Response.Fail("An error occurred while borrowing the book.");
			}
		}

		public async Task<IEnumerable<Borrowing>> GetUserBorrowsAsync(string userId)
		{
			var query = _unitOfWork.Borrowings.GetAllQuery(
							includes: new Expression<Func<Borrowing, object>>[] { b => b.Book }
			);

			query = query.Where(b => b.UserId == userId);

			var result = await query.ToListAsync();

			_logger.LogInformation("Retrieved {Count} borrows for user {UserId}", result.Count, userId);

			return result;
		}

	}
}
