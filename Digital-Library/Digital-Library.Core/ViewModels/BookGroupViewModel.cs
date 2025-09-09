using Digital_Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels
{
	public class BookGroupViewModel
	{
		public decimal TotalSold { get; set; }
		public Book Book { get; set; }
	}
}
