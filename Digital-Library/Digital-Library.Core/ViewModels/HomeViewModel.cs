using Digital_Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels
{
	public class HomeViewModel
	{
		public List<Book> Books { get; set; } = new();
		public List<Category> RandomCategories { get; set; } = new();
	}

}
