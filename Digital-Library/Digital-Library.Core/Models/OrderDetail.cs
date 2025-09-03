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
	public class OrderDetail
	{
		[Key]
		public string Id { get; set; }=Guid.NewGuid().ToString();
		public FormatType FormatType { get; set; }
		public decimal Price { get; set; }
		[Required]
		public int Quantity { get; set; }
		public Order? Order { get; set; }
		[ForeignKey(nameof(Order))]
		public string OrderId { get; set; }
		public Book? Book { get; set; }
		[ForeignKey(nameof(Book))]
		public string BookId { get; set; }
	}
}
