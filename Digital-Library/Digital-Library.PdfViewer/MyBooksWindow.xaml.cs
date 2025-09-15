using Digital_Library.PdfViewer.Enum;
using Digital_Library.PdfViewer.Helper;
using Digital_Library.PdfViewer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Digital_Library.PdfViewer
{
	/// <summary>
	/// Interaction logic for MyBooksWindow.xaml
	/// </summary>
	public partial class MyBooksWindow : Window
	{
		private readonly HttpClient _httpClient;
		private int _currentPage = 1;
		private int _pageSize = 10;
		private int _totalPages = 1;

		public MyBooksWindow()
		{
			InitializeComponent();
			_httpClient = new HttpClient();
			_httpClient.DefaultRequestHeaders.Authorization =
							new AuthenticationHeaderValue("Bearer", UserSession.Instance.JwtToken);

			LoadBooksAsync();
		}
		private async void ReloadButton_Click(object sender, RoutedEventArgs e)
		{
			_currentPage = 1;
			await LoadBooksAsync();
		}
		private async void ShowPdfButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.Tag != null)
			{
				string bookId = (string)btn.Tag;

				ShowPdf(bookId);
			}
		}

		private async Task ShowPdf(string bookId)
		{
			var response = await _httpClient.GetAsync($"https://localhost:7254/api/Client/ShowPdf/{bookId}");
			if (response.IsSuccessStatusCode)
			{
				var pdfBytes = await response.Content.ReadAsByteArrayAsync();
				var tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
				await System.IO.File.WriteAllBytesAsync(tempFilePath, pdfBytes);
				var pdfWindow = new PdfViewerWindow(tempFilePath);
				pdfWindow.ShowDialog();
				try
				{
					System.IO.File.Delete(tempFilePath);
				}
				catch
				{
				}
			}
			else
			{
				MessageBox.Show("Failed to load PDF.");
			}
		}


		private async Task LoadBooksAsync()
		{
			try
			{
				var result = await _httpClient.GetFromJsonAsync<PagedResult<UserBookDto>>(
								$"https://localhost:7254/api/Client/MyBooks?page={_currentPage}&pageSize={_pageSize}");

				if (result != null)
				{
					foreach (var book in result.Items)
					{
						book.ImageBookCoverPath = "https://localhost:7254/" + book.ImageBookCoverPath;

						if (book.StringType == FormatType.Borrowing.ToString())
						{
							if (book.BorrowedUntil.HasValue)
							{
								var daysLeft = (book.BorrowedUntil.Value - DateTime.UtcNow).Days;
								book.BorrowedStatusText = daysLeft > 0 ? $"Borrowed: {daysLeft + 1} day(s) left" : "Expired";
							}
							else
							{
								book.BorrowedStatusText = "Expired";
							}
						}
						else if (book.StringType == FormatType.PDF.ToString())
						{
							book.BorrowedStatusText = "Purchased";
						}
					}

					BooksItemsControl.ItemsSource = result.Items;
					_totalPages = (int)Math.Ceiling((double)result.TotalCount / _pageSize);
					PageInfoText.Text = $"Page {_currentPage} of {_totalPages}";

					PrevButton.IsEnabled = _currentPage > 1;
					NextButton.IsEnabled = _currentPage < _totalPages;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error loading books: " + ex.Message);
			}
		}





		private async void PrevButton_Click(object sender, RoutedEventArgs e)
		{
			if (_currentPage > 1)
			{
				_currentPage--;
				await LoadBooksAsync();
			}
		}

		private async void NextButton_Click(object sender, RoutedEventArgs e)
		{
			if (_currentPage < _totalPages)
			{
				_currentPage++;
				await LoadBooksAsync();
			}
		}
	}
}
