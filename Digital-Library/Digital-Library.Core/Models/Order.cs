using Digital_Library.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public Status Status { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

    }
}
