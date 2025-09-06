using Digital_Library.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Requests
{
    public class CartDetailRequest
    {
        [Required]
        public int Quantity { get; set; }
        [Required]
        public FormatType FormatType { get; set; }

        [Required]
        public string BookId { get; set; }

        // Optional (only for borrow)
        public int? Days { get; set; }
    }
}
