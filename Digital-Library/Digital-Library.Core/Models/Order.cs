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
	public class Order
	{
		[Key]
		public string Id { get; set; }=	Guid.NewGuid().ToString();
		public DateTime OrderDate { get; set; }=	DateTime.Now;
		[Required]
		public decimal TotalAmount { get; set; }
		public Status Status { get; set; }=Status.Pending;

		public ICollection<Transaction>? Transactions { get; set; }

		public ICollection<OrderDetail>? OrderDetails { get; set; }

		public User? User { get; set; }
		[ForeignKey(nameof(User))]
		[Required]
		public string UserId { get; set; }

	}
}
