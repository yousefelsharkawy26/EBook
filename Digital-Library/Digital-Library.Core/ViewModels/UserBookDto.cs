using Digital_Library.Core.Enums;

namespace Digital_Library.Core.ViewModels;
public class UserBookDto
{
	public string BookId { get; set; }
	public string Title { get; set; }
	public string Author { get; set; }
	public FormatType Type { get; set; }
	public string ImageBookCoverPath { get; set; }
	public DateTime? BorrowedUntil { get; set; }
}
