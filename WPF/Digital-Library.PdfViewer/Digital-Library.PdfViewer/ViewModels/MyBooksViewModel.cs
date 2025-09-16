using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Digital_Library.PdfViewer.Enum;
using Digital_Library.PdfViewer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Windows.Input;

namespace Digital_Library.PdfViewer.ViewModels
{
    public partial class MyBooksViewModel : ObservableObject
    {
        private readonly IBookService _bookService;
        private readonly IServiceProvider _serviceProvider;
        private int _pageSize = 10;

        // Properties for Data Binding
        public ObservableCollection<BookItemViewModel> Books { get; } = new ObservableCollection<BookItemViewModel>();

        [ObservableProperty] private int _currentPage = 1;

        [ObservableProperty] private int _totalPages;


        [ObservableProperty] private string _pageInfoText;
       

        [ObservableProperty] private bool _isLoading;


        [ObservableProperty] private string _errorMessage;
        

        // Commands for Actions
        

        // Event to ask the View to show the PDF window
        public event Action<MemoryStream, string, FormatType> ShowPdfRequested;

        // Constructor
        public MyBooksViewModel(IBookService bookService, 
                                IServiceProvider serviceProvider)
        {
            _bookService = bookService;
            _serviceProvider = serviceProvider;

            // Load initial data
            LoadBooksCommand.Execute(null);
        }

        // Default constructor for XAML designer

        [RelayCommand]
        private async Task LoadBooksAsync()
        {
            var config = _serviceProvider.GetRequiredService<IConfiguration>();

            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var result = await _bookService.GetMyBooksAsync(CurrentPage, _pageSize);

                Books.Clear();
                foreach (var bookDto in result.Items)
                {
                    Books.Add(new BookItemViewModel(bookDto, config));
                }

                TotalPages = (int)Math.Ceiling((double)result.TotalCount / _pageSize);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading books: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ShowPdfForBookAsync(BookItemViewModel book)
        {
            if (book == null) return;

            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var (decryptedPdf, email, type) = await _bookService.GetAndDecryptPdfAsync(book.BookId);

                // Use a copy so the original can be disposed
                var pdfStream = new MemoryStream(decryptedPdf);

                // Ask the View to handle the UI part
                ShowPdfRequested?.Invoke(pdfStream, email, type);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error displaying PDF: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        bool CanGoToNextPage() => CurrentPage < TotalPages && !IsLoading;

        [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
        private async Task GoToNextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadBooksAsync();
            }
        }
        bool CanGoToPrevPage() => CurrentPage > 1 && !IsLoading;
        [RelayCommand(CanExecute = nameof(CanGoToPrevPage))]
        private async Task GoToPrevPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadBooksAsync();
            }
        }
        private void UpdatePageInfo()
        {
            PageInfoText = $"Page {CurrentPage} of {TotalPages}";
        }
    }
}