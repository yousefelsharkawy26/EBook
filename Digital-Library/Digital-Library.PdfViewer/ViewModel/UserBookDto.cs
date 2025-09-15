using Digital_Library.PdfViewer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Digital_Library.PdfViewer.ViewModel
{
	public class UserBookDto
	{
		public string BookId { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string StringType { get; set; }
		public string ImageBookCoverPath { get; set; }
		public DateTime? BorrowedUntil { get; set; }

		public bool IsOwned { get; set; }
		public int? BorrowedDaysLeft { get; set; }
		public string BorrowedStatusText { get; set; } = string.Empty;

	}

}
