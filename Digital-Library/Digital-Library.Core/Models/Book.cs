using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Digital_Library.Core.Models;
public class Book
{
	[Key]
	public string Id { get; set; } = Guid.NewGuid().ToString();
	[Required]
	public string Title { get; set; }
	[Required]
	public string Author { get; set; }
	[Required]
	public decimal PricePhysical { get; set; }
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
	public string? PDFFilePath { get; set; }
	public DateTime UploadDate { get; set; }= DateTime.Now;

	public Category? Category { get; set; }
	[ForeignKey(nameof(Category))]
	public string CategoryID { get; set; }

	public ICollection<Borrowing>? Borrowing { get; set; }
	public ICollection<CartDetail>? CartDetails { get; set; }
	public ICollection<OrderDetail>? OrderDetails { get; set; }

	public Vendor? Vendor { get; set; }
	[ForeignKey(nameof(Vendor))]
	public string VendorId { get; set; }



}
