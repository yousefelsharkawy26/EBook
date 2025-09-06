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


		public ICollection<OrderHeader>? OrderHeaders { get; set; }
		public User? User { get; set; }
		[ForeignKey(nameof(User))]
		[Required]
		public string UserId { get; set; }
		[Required]
		[Phone]
		public string PhoneNumber { get; set; }

		[Required]
		public string Address { get; set; }

	}
}
