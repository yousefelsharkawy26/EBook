using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
	public class Cart
	{
		[Key]
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public ICollection<CartDetail>? CartDetails { get; set; }
		public User? User { get; set; }
		[ForeignKey(nameof(User))]
		[Required]
		public string UserId { get; set; }
	}
}
