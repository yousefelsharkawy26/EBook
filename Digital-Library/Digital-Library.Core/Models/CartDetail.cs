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
	public class CartDetail
	{
		[Key]
		public string Id { get; set; } = Guid.NewGuid().ToString();
		[Required]
		public int Quantity { get; set; }
		[Required]
		public FormatType FormatType { get; set; }

		[ForeignKey(nameof(Cart))]
		[Required]
		public string CartId { get; set; }
		public Cart? Cart { get; set; }
		[ForeignKey(nameof(Book))]
		[Required]
		public string BookId { get; set; }
		public Book? Book { get; set; }


	}
}
