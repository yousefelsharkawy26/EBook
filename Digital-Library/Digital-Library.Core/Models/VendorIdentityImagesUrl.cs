using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
	public class VendorIdentityImagesUrl
	{
		[Key]
		public string Id { get; set; }=	Guid.NewGuid().ToString();
		public string Url { get; set; }
		[ForeignKey(nameof(Vendor))]
		public string VendorId { get; set; }
		public Vendor Vendor { get; set; }
	}
}
