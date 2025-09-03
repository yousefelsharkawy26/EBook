using Digital_Library.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
    public class CartDetail
    {
        public Guid CartDetailId { get; set; }
        public int Quantity { get; set; }
        public FormatType FormatType { get; set; }
        
    }
}
