using System.ComponentModel.DataAnnotations;

namespace Digital_Library.Core.Models;
public class Category
{
	[Key]
	public string Id { get; set; } = Guid.NewGuid().ToString();
	[Required]
	public string CategoryName { get; set; }
	[Required]
	public string Description { get; set; }
	[Required]
	public bool IsApproved { get; set; }

	public ICollection<Book>? Books { get; set; }
}
