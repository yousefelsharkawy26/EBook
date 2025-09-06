using Digital_Library.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
	public class OrderHeader
	{
		[Key]
		public string Id { get; set; }=	Guid.NewGuid().ToString();
		public Order? Order { get; set; }
		[ForeignKey(nameof(Order))]
		[Required]
		public string OrderId { get; set; }

		public Vendor? Vendor { get; set; }

		[ForeignKey(nameof(Vendor))]
		[Required]
		public string VendorId { get; set; }

		public Status Status { get; set; } = Status.Pending;

		public ICollection<OrderDetail>? OrderDetails { get; set; }
		public ICollection<Transaction>? Transactions { get; set; }

	}
}
