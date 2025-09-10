// Core/ViewModels/BookSummaryViewModel.cs
namespace Digital_Library.Core.ViewModels;

// في Core/ViewModels/BookSummaryViewModel.cs
public class BookSummaryViewModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string VendorName { get; set; }
    public decimal PricePhysical { get; set; }
    public string CoverImageUrl { get; set; }
}