using Digital_Library.Core.Models;

namespace Digital_Library.Core.ViewModels;
public class BookDetailsViewModel
{
    public Book Book { get; set; }
    public IEnumerable<Book> RelatedBooks { get; set; }
}
