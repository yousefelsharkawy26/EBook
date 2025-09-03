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


		public decimal Amount { get; set; }

		public PaymentMethod PaymentMethod { get; set; }

		public DateTime TransactionDate { get; set; }=	DateTime.Now;

		public string ReferenceCode { get; set; }
		[ForeignKey(nameof(Order)]
		public string OrderId { get; set; }
		public Order? Order { get; set; }



	}
}
