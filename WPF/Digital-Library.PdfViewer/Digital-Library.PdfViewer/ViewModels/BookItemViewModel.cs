using CommunityToolkit.Mvvm.ComponentModel;
using Digital_Library.PdfViewer.Enum;
using Digital_Library.PdfViewer.Models;
using Microsoft.Extensions.Configuration;

namespace Digital_Library.PdfViewer.ViewModels;
public partial class BookItemViewModel : ObservableObject // Assumes ViewModelBase from previous example
{
    private readonly UserBookDto _book;

    public string BookId => _book.BookId;
    public string Title => _book.Title;
    public string Author => _book.Author;
    [ObservableProperty] private string _fullImageBookCoverPath;
    [ObservableProperty] private string _borrowedStatusText;

    public BookItemViewModel(UserBookDto book, IConfiguration config)
    {
        _book = book;

        // Presentation logic is handled here, during object creation
        FullImageBookCoverPath = $"{config["BaseURL"]}{_book.ImageBookCoverPath}";

        if (_book.StringType == FormatType.Borrowing.ToString())
        {
            if (_book.BorrowedUntil.HasValue)
            {
                var daysLeft = (_book.BorrowedUntil.Value.Date - DateTime.UtcNow.Date).Days;
                BorrowedStatusText = daysLeft >= 0 ? $"Borrowed: {daysLeft + 1} day(s) left" : "Expired";
            }
            else
            {
                BorrowedStatusText = "Expired";
            }
        }
        else if (_book.StringType == FormatType.PDF.ToString())
        {
            BorrowedStatusText = "Purchased";
        }
    }
}