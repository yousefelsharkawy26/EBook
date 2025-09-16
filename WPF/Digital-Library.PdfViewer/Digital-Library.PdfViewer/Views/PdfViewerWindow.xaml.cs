using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Microsoft.Win32;
using System.Windows.Ink;
using Digital_Library.PdfViewer.Enum;

namespace Digital_Library.PdfViewer.Views
{
	public partial class PdfViewerWindow : Window
	{
		private byte[] _originalPdfBytes;
		private string _currentType;

		private static class NativeMethods
		{
			[DllImport("user32.dll")]
			public static extern uint SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);
			public const uint WDA_MONITOR = 0x00000001;
			public const uint WDA_NONE = 0x00000000;
		}

		public PdfViewerWindow(MemoryStream pdfStream, string watermarkText, string type)
		{
			InitializeComponent();
			_originalPdfBytes = pdfStream.ToArray();
			this.Loaded += (s, e) => LoadPdfAndApplySecurity(watermarkText, type);
		}

		private void LoadPdfAndApplySecurity(string watermarkText, string type)
		{
			try
			{
				_currentType = type;
				PdfRenderer.Load(_originalPdfBytes);

				UpdatePageInfo();

				if (type == FormatType.PDF.ToString())
				{
					this.Title = "Document Editor";
					PdfToolBar.Visibility = Visibility.Visible;
					Select_Click(null, null);
				}
				else if (type == FormatType.Borrowing.ToString())
				{
					this.Title = "Borrowed Document - Read Only";
					PdfToolBar.Visibility = Visibility.Collapsed;
				}

				AddWatermark((Panel)MainGrid, watermarkText);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to load PDF: " + ex.Message);
				this.Close();
			}
		}

		#region Toolbar Buttons

		private void Prev_Click(object sender, RoutedEventArgs e)
		{
			PdfRenderer.PrevPage();
			UpdatePageInfo();
		}

		private void Next_Click(object sender, RoutedEventArgs e)
		{
			PdfRenderer.NextPage();
			UpdatePageInfo();
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Right)
			{
				PdfRenderer.NextPage();
				UpdatePageInfo();
			}
			else if (e.Key == Key.Left)
			{
				PdfRenderer.PrevPage();
				UpdatePageInfo();
			}
		}
		private void Pencil_Click(object sender, RoutedEventArgs e)
		{
			var canvas = PdfRenderer.GetCurrentCanvas();
			canvas.IsHitTestVisible = true;
			canvas.EditingMode = InkCanvasEditingMode.Ink;
			canvas.DefaultDrawingAttributes = new DrawingAttributes
			{
				Color = Colors.Blue,
				Width = 2,
				Height = 2,
				IsHighlighter = false
			};
		}

		private void Highlight_Click(object sender, RoutedEventArgs e)
		{
			var canvas = PdfRenderer.GetCurrentCanvas();
			canvas.IsHitTestVisible = true;
			canvas.EditingMode = InkCanvasEditingMode.Ink;
			canvas.DefaultDrawingAttributes = new DrawingAttributes
			{
				Color = Colors.Yellow,
				Width = 15,
				Height = 15,
				IsHighlighter = true
			};
		}

		private void Text_Click(object sender, RoutedEventArgs e)
		{
			var canvas = PdfRenderer.GetCurrentCanvas();
			var textBox = new TextBox
			{
				Width = 150,
				Height = 30,
				Background = Brushes.Transparent,
				Foreground = Brushes.Red,
				BorderThickness = new Thickness(0),
				FontSize = 16,
				AcceptsReturn = true
			};

			Canvas.SetLeft(textBox, 100);
			Canvas.SetTop(textBox, 100);

			canvas.Children.Add(textBox);
		}

		private void Erase_Click(object sender, RoutedEventArgs e)
		{
			var canvas = PdfRenderer.GetCurrentCanvas();
			canvas.IsHitTestVisible = true;
			canvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
		}

		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			var canvas = PdfRenderer.GetCurrentCanvas();
			canvas.Strokes.Clear();

			for (int i = canvas.Children.Count - 1; i >= 0; i--)
			{
				if (canvas.Children[i] is TextBox)
					canvas.Children.RemoveAt(i);
			}
		}

		private void Select_Click(object sender, RoutedEventArgs e)
		{
			var canvas = PdfRenderer.GetCurrentCanvas();
			canvas.IsHitTestVisible = false;
		}

		private void ZoomIn_Click(object sender, RoutedEventArgs e) => PdfRenderer.ZoomIn();
		private void ZoomOut_Click(object sender, RoutedEventArgs e) => PdfRenderer.ZoomOut();

		private void SearchBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (int.TryParse(SearchBox.Text, out int pageNum))
				{
					if (pageNum >= 1 && pageNum <= PdfRenderer.PageCount)
					{
						PdfRenderer.ShowPage(pageNum - 1);
						UpdatePageInfo();
					}
				}
			}
		}

		#endregion

		private void UpdatePageInfo()
		{
			PageInfo.Text = $"Page {PdfRenderer.CurrentPage} / {PdfRenderer.PageCount}";
		}

		private void AddWatermark(Panel parentPanel, string text)
		{
			var watermark = new Label
			{
				Content = text,
				FontSize = 32,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Color.FromArgb(40, 255, 0, 0)),
				RenderTransform = new RotateTransform(-45),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				IsHitTestVisible = false
			};
			Panel.SetZIndex(watermark, 100);
			parentPanel.Children.Add(watermark);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				var windowHandle = new WindowInteropHelper(this).Handle;
				NativeMethods.SetWindowDisplayAffinity(windowHandle, NativeMethods.WDA_MONITOR);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to enable screen capture protection: " + ex.Message);
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			try
			{
				var windowHandle = new WindowInteropHelper(this).Handle;
				NativeMethods.SetWindowDisplayAffinity(windowHandle, NativeMethods.WDA_NONE);
			}
			catch { }

			base.OnClosed(e);
		}
	}
}
