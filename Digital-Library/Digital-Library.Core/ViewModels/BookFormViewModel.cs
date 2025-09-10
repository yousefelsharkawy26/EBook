// Core/ViewModels/BookFormViewModel.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Digital_Library.Core.ViewModels;

// في Core/ViewModels/BookFormViewModel.cs
public class BookFormViewModel
{
    public string Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; }

    [Required, StringLength(100)]
    public string Author { get; set; }

    [Required]
    public string Description { get; set; }

    [Display(Name = "Price (Physical)")]
    public decimal PricePhysical { get; set; }

    [Display(Name = "Price (PDF)")]
    public decimal PricePdf { get; set; }

    [ValidateNever]
    public IFormFile? CoverImage { get; set; }

    [Required, Display(Name = "Category")]
    public string CategoryId { get; set; }

    [Required, Display(Name = "Vendor")]
    public string VendorId { get; set; }

    // هذه القوائم ستُستخدم لملء الـ Dropdowns في النموذج
    public IEnumerable<SelectListItem>? Categories { get; set; }
    public IEnumerable<SelectListItem>? Vendors { get; set; }
}