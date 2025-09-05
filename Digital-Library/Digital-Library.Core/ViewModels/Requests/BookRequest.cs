using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Requests
{
	public class BookRequest : IValidatableObject
	{
		[Required(ErrorMessage = "Title is required")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Author is required")]
		public string Author { get; set; }

		[Required(ErrorMessage = "Physical book price is required")]
		[Range(1, double.MaxValue, ErrorMessage = "Physical price must be greater than 0")]
		public decimal PricePhysical { get; set; }

		public decimal? PricePDF { get; set; }   
		public decimal? PricePDFPerDay { get; set; } 

		[Required(ErrorMessage = "Description is required")]
		public string Description { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Stock must be valid number")]
		public int Stock { get; set; }

		[Display(Name = "Has PDF Version?")]
		public bool HasPDF { get; set; }

		[Display(Name = "Is Borrowable?")]
		public bool IsBorrowable { get; set; }

		[Required(ErrorMessage = "Category is required")]
		public string CategoryID { get; set; }

		public IFormFile? PDFFile { get; set; }

		[Required(ErrorMessage = "Book cover image is required")]
		public IFormFile ImageBookCover { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (HasPDF)
			{
				if (PDFFile == null)
				{
					yield return new ValidationResult(
									"You must upload a PDF file if the book has PDF.",
									new[] { nameof(PDFFile) });
				}

				if (PricePDF == null || PricePDF <= 0)
				{
					yield return new ValidationResult(
									"You must provide a valid price for PDF purchase.",
									new[] { nameof(PricePDF) });
				}

				if (IsBorrowable && (PricePDFPerDay == null || PricePDFPerDay <= 0))
				{
					yield return new ValidationResult(
									"You must provide a valid daily price for PDF borrowing.",
									new[] { nameof(PricePDFPerDay) });
				}
			}
		}
	}

}
