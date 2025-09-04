using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Requests
{
	public class VendorUpdateRequest
	{
		[Required]
		public string LibraryName { get; set; }

		[Required]
		public string City { get; set; }

		[Required]
		public string State { get; set; }

		[Required]
		public string ZipCode { get; set; }

		[Required]
		[Phone]
		public string ContactNumber { get; set; }

		public IFormFile? LibraryLogo { get; set; }
	}
}
