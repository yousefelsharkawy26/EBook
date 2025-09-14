using Digital_Library.Core.ViewModels;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Digital_Library.AdminPanel.Controllers
{
	public class VendorController : Controller
	{
		private readonly IVendorService _vendorService;

		public VendorController(IVendorService vendorService)
		{
			_vendorService = vendorService;
		}


		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> NewVendors()
		{
			var vendorViewModels = await _vendorService.GetVendorsDetailsAsync();

			return View(vendorViewModels);
		}
	}
}
