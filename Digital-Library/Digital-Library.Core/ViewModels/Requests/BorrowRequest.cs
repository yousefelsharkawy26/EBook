using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Requests
{
	public class BorrowRequest
	{
		[Required]
		public string BookId { get; set; }
		[Required]
		public int Days { get; set; }
	}
}
