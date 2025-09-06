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
		[Required]
		public FormatType FormatType { get; set; }
		[Required]
		public decimal Price { get; set; }
		[Required]
		public int Quantity { get; set; }
		public Book? Book { get; set; }
		[ForeignKey(nameof(Book))]
		[Required]
		public string BookId { get; set; }
		public OrderHeader? OrderHeader { get; set; }
		[ForeignKey(nameof(OrderHeader))]
		[Required]
		public int OrderHeaderId { get; set; }
	}
}
