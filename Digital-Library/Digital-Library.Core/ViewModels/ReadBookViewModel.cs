using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels
{
	public class ReadBookViewModel
	{
		public string BookId { get; set; }
		public string Title { get; set; }
		public string FilePath { get; set; }
		public bool IsBorrowed { get; set; }
		public bool CanDownload { get; set; }
		public bool CanPrint { get; set; }
	}

}
