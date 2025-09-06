using Digital_Library.Core.Enum;
using Digital_Library.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
	public class Transaction
	{
		[Key]
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public Status TransactionStatus { get; set; }
		[Required]
		public decimal Amount { get; set; }
		public PaymentMethod PaymentMethod { get; set; }
		public DateTime TransactionDate { get; set; }=	DateTime.Now;
		public string ReferenceCode { get; set; }=	Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
		[ForeignKey(nameof(Order))]
		[Required]
		public string OrderHeaderId { get; set; }
		public OrderHeader? OrderHeader { get; set; }



	}
}
