using Digital_Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels
{
	public class SearchBarViewModel
	{
		public IEnumerable<Category> Categories { get; set; } 
	}

}
