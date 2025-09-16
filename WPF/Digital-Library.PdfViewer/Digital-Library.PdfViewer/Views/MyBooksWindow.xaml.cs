using Digital_Library.PdfViewer.Enum;
using Digital_Library.PdfViewer.ViewModels;
using System.IO;
using System.Windows;

namespace Digital_Library.PdfViewer.Views
{
	public partial class MyBooksWindow : Window
	{
		
		public MyBooksWindow(MyBooksViewModel viewModel)
		{
			InitializeComponent();
            // The DataContext is set in XAML. We just need to hook into its events.
            DataContext = viewModel;
            viewModel.ShowPdfRequested += OnShowPdfRequested;
        }

        private void OnShowPdfRequested(MemoryStream pdfStream, string email, FormatType type)
        {
            // The ViewModel has done all the work. The View just displays the result.
            var pdfWindow = new PdfViewerWindow(pdfStream, email, type.ToString());

            pdfWindow.ShowDialog();
        }
    }
}
