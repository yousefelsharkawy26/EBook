using Digital_Library.Core.Models;

namespace Digital_Library.Core.ViewModels;
public class HomeViewModel
{
	public List<Book> Books { get; set; } = new();
	public List<Category> RandomCategories { get; set; } = new();
}
