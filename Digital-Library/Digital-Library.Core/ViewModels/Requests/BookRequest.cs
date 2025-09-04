using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Requests
{
	public class BookRequest
	{
		[Required]
		public string Title { get; set; }
		[Required]
		public string Author { get; set; }
		[Required]
		public decimal PricePhysical { get; set; } = 0;
		[Required]
		public decimal PricePDFPerDay { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public int Stock { get; set; }
		[Required]
		public bool HasPDF { get; set; }
		[Required]
		public bool IsBorrowable { get; set; }
		[Required]
		public string CategoryID { get; set; }
		public IFormFile? PDFFile { get; set; }


	}
}
