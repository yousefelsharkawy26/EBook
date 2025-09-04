using Digital_Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Interface
{
    public interface IBorrowService
    {
        Task BorrowBookAsync(string userId, string bookId , int days = 14);
        Task ReturnBookAsync(string userId, string bookId);

        Task<IEnumerable<Borrowing>> GetUserBorrowsAsync(string userId);
    }
}
