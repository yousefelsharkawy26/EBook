using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
    public class OrderDetails
    {
        public Guid OrderDetailsId { get; set; }
        //public FormatType FormatType { get; set; }
        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
