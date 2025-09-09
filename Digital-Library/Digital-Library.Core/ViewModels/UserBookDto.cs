using Digital_Library.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels
{
	public class UserBookDto
	{
		public string BookId { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public FormatType Type { get; set; } 
	}

}
