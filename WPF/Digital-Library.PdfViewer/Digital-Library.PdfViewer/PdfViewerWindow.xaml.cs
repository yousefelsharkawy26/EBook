using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Threading.Tasks;

namespace Digital_Library.PdfViewer
{
	public partial class PdfViewerWindow : Window
	{
		// متغير لحفظ مسار الملف المؤقت
		private string _tempPdfPath;

		// --- كود منع تصوير الشاشة (يعمل على مستوى النافذة) ---
		private static class NativeMethods
		{
			[DllImport("user32.dll")]
			public static extern uint SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);
			public const uint WDA_MONITOR = 0x00000001; // يمنع التصوير والتسجيل
			public const uint WDA_NONE = 0x00000000;
		}

		public PdfViewerWindow(MemoryStream pdfStream, string watermarkText)
		{
			InitializeComponent();
			LoadPdfAndApplySecurityAsync(pdfStream, watermarkText);
		}

		private async Task LoadPdfAndApplySecurityAsync(MemoryStream pdfStream, string watermarkText)
		{
			try
			{
				// 1. إنشاء ملف PDF مؤقت لأن WebView2 يعمل بشكل أفضل مع الملفات
				_tempPdfPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
				await File.WriteAllBytesAsync(_tempPdfPath, pdfStream.ToArray());

				// 2. تهيئة WebView2
				await WebView.EnsureCoreWebView2Async(null);

				// 3. عرض الـ PDF مع إخفاء شريط الأدوات (وهذا يمنع الطباعة والحفظ)
				// الحيلة هي إضافة #toolbar=0 إلى نهاية مسار الملف
				WebView.CoreWebView2.Navigate($"file:///{_tempPdfPath}#toolbar=0");

				// 4. إضافة العلامة المائية فوق العارض
				AddWatermark(MainGrid, watermarkText);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to load PDF: " + ex.Message);
				this.Close();
			}
		}

		private void AddWatermark(Grid parentGrid, string text)
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
				IsHitTestVisible = false // للسماح بالتفاعل مع الـ PDF في الخلف
			};
			parentGrid.Children.Add(watermark);
		}

		// --- تفعيل الحماية عند تحميل النافذة ---
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				var windowHandle = new WindowInteropHelper(this).Handle;
				// تفعيل الحماية من تصوير الشاشة وتسجيلها
				NativeMethods.SetWindowDisplayAffinity(windowHandle, NativeMethods.WDA_MONITOR);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to enable screen capture protection: " + ex.Message);
			}
		}

		// --- تنظيف الموارد عند إغلاق النافذة ---
		protected override void OnClosed(EventArgs e)
		{
			// إيقاف الحماية (خطوة اختيارية ولكنها جيدة)
			try
			{
				var windowHandle = new WindowInteropHelper(this).Handle;
				NativeMethods.SetWindowDisplayAffinity(windowHandle, NativeMethods.WDA_NONE);
			}
			catch { /* تجاهل الأخطاء */ }

			// التخلص من WebView2 لتحرير الملف المؤقت
			WebView?.Dispose();

			// حذف الملف المؤقت (مهم جداً)
			if (!string.IsNullOrEmpty(_tempPdfPath) && File.Exists(_tempPdfPath))
			{
				try
				{
					File.Delete(_tempPdfPath);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Could not delete temp file: " + ex.Message);
				}
			}
			base.OnClosed(e);
		}
	}
}