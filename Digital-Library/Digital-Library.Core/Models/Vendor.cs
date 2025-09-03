using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
	public class Vendor
	{
		[Key]
		public string Id { get; set; }=	Guid.NewGuid().ToString();
		[Required]
		public string LibraryName { get; set; }
		[Required]
		public string City { get; set; }
		[Required]
		public string State { get; set; }
		[Required]
		public string ZipCode { get; set; }
		[Required]
		public string ContactNumber { get; set; }
		public decimal WalletBalance { get; set; }
		public ICollection<Book>? Books { get; set; }
		public ICollection<VendorIdentityImagesUrl>? vendorIdentityImagesUrls { get; set; }
		public User? User { get; set; }
		[ForeignKey(nameof(User))]
		public string UserId { get; set; }




	}
}
