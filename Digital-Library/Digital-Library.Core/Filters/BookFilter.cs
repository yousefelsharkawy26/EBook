using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Filters
{
	public class BookFilter
	{
		public string? VendorId { get; set; }
		public string? CategoryId { get; set; }
		public bool? HasPDF { get; set; }
		public bool? IsBorrowable { get; set; }
		public string? Keyword { get; set; }  
	}
}
