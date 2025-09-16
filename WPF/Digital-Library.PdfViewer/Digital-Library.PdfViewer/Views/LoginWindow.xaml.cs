using Digital_Library.PdfViewer.Helpers;
using Digital_Library.PdfViewer.Models;
using Digital_Library.PdfViewer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Digital_Library.PdfViewer.Views
{
	public partial class LoginWindow : Window
	{
        private readonly IServiceProvider _serviceProvider;
        public LoginWindow(IServiceProvider serviceProvider,
                           LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.LoginSucceeded += OnLoginSucceeded;
            _serviceProvider = serviceProvider;
        }

        private void OnLoginSucceeded(object? sender, System.EventArgs e)
        {
            // This is the ONLY logic left in the code-behind:
            // Responding to a signal from the ViewModel to do something View-specific.
            var myBooksWindow = _serviceProvider.GetRequiredService<MyBooksWindow>();
            
            myBooksWindow.Show();
            this.Close();
        }
    }
}
