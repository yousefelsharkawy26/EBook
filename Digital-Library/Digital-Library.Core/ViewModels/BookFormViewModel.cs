// Core/ViewModels/BookFormViewModel.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Digital_Library.Core.ViewModels;

public class BookFormViewModel
{
	public string Id { get; set; }

	public string Title { get; set; }
	public string Author { get; set; }
	public string Description { get; set; }

	public decimal PricePhysical { get; set; }
	public decimal PricePdf { get; set; }

	// صورة قديمة (لو موجودة)
	public string? ExistingCoverImage { get; set; }
	public IFormFile? CoverImage { get; set; }

	public string CategoryId { get; set; }
	public string VendorId { get; set; }

	public IEnumerable<SelectListItem>? Categories { get; set; }
	public IEnumerable<SelectListItem>? Vendors { get; set; }
}