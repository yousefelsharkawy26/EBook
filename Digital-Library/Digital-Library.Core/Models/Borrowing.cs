using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
    internal class Borrowing
    {
        public Guid Id { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime DueDate{ get; set; }

        public Book Book { get; set; }


        public Guid BookId { get; set; }
    }
}
