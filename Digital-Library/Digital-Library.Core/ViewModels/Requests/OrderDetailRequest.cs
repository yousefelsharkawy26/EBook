using Digital_Library.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Requests
{
	public class OrderDetailRequest
	{
		[Required]
		public FormatType FormatType { get; set; }
		[Required]
		public decimal Price { get; set; }
		[Required]
		public int Quantity { get; set; }
		[Required]
		public string BookId { get; set; }
		[Required]
		public string VendorId { get; set; }
	}
}
