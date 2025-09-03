using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Digital_Library.Core.Models;

public class Borrowing
{
	[Key]
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public DateTime BorrowDate { get; set; }=	DateTime.Now;
	[Required]
	public DateTime DueDate { get; set; }
	public Book? Book { get; set; }
	[ForeignKey(nameof(Book))]
	[Required]
	public string BookId { get; set; }

	public User? User { get; set; }
	[ForeignKey(nameof(User))]
	[Required]
	public string UserId { get; set; }
}
