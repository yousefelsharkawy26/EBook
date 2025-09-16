namespace Digital_Library.PdfViewer.Models;
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
