namespace Digital_Library.Core.Models;

public class Borrowing
{
    public Guid Id { get; set; }

    public DateTime BorrowDate { get; set; }

    public DateTime DueDate{ get; set; }

    public Book Book { get; set; }


    public Guid BookId { get; set; }
}
