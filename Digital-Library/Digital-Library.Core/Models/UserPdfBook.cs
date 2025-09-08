using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
	public class UserPdfBook
	{
		[ForeignKey(nameof(User))]
		public string UserId { get; set; }
		[ForeignKey(nameof(Book))]
		public string BookId { get; set; }

		public User? User { get; set; }

		public Book? Book { get; set; }


	}
}
