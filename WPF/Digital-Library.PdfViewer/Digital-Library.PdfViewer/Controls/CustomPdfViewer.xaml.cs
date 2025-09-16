using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Docnet.Core;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using System.Windows;

namespace Digital_Library.PdfViewer.Controls
{
	public partial class CustomPdfViewer : UserControl
	{
		private readonly List<BitmapSource> _pages = new();
		private readonly List<InkCanvas> _canvases = new();
		private IDocReader _docReader;
		private int _currentPageIndex = 0;
		private double _zoom = 1.0;

		public int PageCount => _pages.Count;
		public int CurrentPage => _currentPageIndex + 1;

		public CustomPdfViewer()
		{
			InitializeComponent();
		}

		public void Load(byte[] pdfBytes)
		{
			_pages.Clear();
			_canvases.Clear();
			_docReader?.Dispose();
			_docReader = DocLib.Instance.GetDocReader(pdfBytes, new PageDimensions(1080, 1920));

			for (var i = 0; i < _docReader.GetPageCount(); i++)
			{
				using var pageReader = _docReader.GetPageReader(i);
				var rawBytes = pageReader.GetImage();
				var width = pageReader.GetPageWidth();
				var height = pageReader.GetPageHeight();
				var stride = width * 4;

				var bmpSource = BitmapSource.Create(
								width, height, 96, 96,
								PixelFormats.Bgra32,
								null, rawBytes, stride);

				_pages.Add(bmpSource);

				var canvas = new InkCanvas
				{
					Background = Brushes.Transparent,
					Width = width,
					Height = height
				};
				_canvases.Add(canvas);
			}

			ShowPage(0);
		}

		public void ShowPage(int index)
		{
			if (index >= 0 && index < _pages.Count)
			{
				_currentPageIndex = index;

				var grid = new Grid();

				// صورة الصفحة
				grid.Children.Add(new Image { Source = _pages[_currentPageIndex] });

				// افصل الـ InkCanvas من Parent القديم لو موجود
				var inkCanvas = _canvases[_currentPageIndex];
				if (inkCanvas.Parent is Panel oldParent)
				{
					oldParent.Children.Remove(inkCanvas);
				}

				// أضف الـ InkCanvas للـ Grid الجديد
				grid.Children.Add(inkCanvas);

				grid.LayoutTransform = new ScaleTransform(_zoom, _zoom);

				PagePresenter.Content = grid;

				// ✅ تحديث العداد
				PageInfo.Text = $"Page {CurrentPage} / {PageCount}";
			}
		}


		public void NextPage()
		{
			if (_currentPageIndex + 1 < _pages.Count)
			{
				ShowPage(_currentPageIndex + 1);
			}
		}

		public void PrevPage()
		{
			if (_currentPageIndex - 1 >= 0)
			{
				ShowPage(_currentPageIndex - 1);
			}
		}

		public InkCanvas GetCurrentCanvas() => _canvases[_currentPageIndex];

		public void ZoomIn()
		{
			_zoom += 0.1;
			ApplyZoom();
		}

		public void ZoomOut()
		{
			if (_zoom > 0.2)
				_zoom -= 0.1;
			ApplyZoom();
		}

		private void ApplyZoom()
		{
			if (PagePresenter.Content is Grid grid)
			{
				grid.LayoutTransform = new ScaleTransform(_zoom, _zoom);
			}
		}
	}
}
