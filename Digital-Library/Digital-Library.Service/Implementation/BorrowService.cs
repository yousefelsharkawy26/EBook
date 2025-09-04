using Digital_Library.Core.Models;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Implementation
{
    public class BorrowService : IBorrowService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BorrowService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task BorrowBookAsync(string userId, string bookId , int days)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(bookId);

            if (book == null )
            {
                throw new KeyNotFoundException($"Book not found ");
            }

            if ( !book.IsBorrowable )
            {
                throw new KeyNotFoundException($"Book is already borrowed");
            }

            if (book.Stock == 0)
            {
                book.IsBorrowable = false;
                return;
            }

            book.Stock--;

            var borrowing = new Borrowing()
            {
                UserId = userId,
                BookId = bookId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(days),
            };
            await _unitOfWork.Borrowings.AddAsync(borrowing);
            _unitOfWork.Books.Update(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<Borrowing>> GetUserBorrowsAsync(string userId)
        {
            return await _unitOfWork.Borrowings.GetManyAsync(b => b.UserId == userId, b => b.Book);
        }
    }
}
