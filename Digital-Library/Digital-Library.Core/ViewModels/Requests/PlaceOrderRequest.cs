using Digital_Library.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Requests
{
	public class PlaceOrderRequest
	{
		[Required]
		public string Address { get; set; }
		[Required]
		public string PhoneNumber { get; set; }

		[Required]
		public PaymentMethod PaymentMethod { get; set; }
	}
}
