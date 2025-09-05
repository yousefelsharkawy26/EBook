using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Digital_Library.Controllers
{
	[Route("vendor")]
	[Authorize]
	public class VendorController : Controller
	{
		private readonly IVendorService vendorService;

		public VendorController(IVendorService vendorService)
		{
			this.vendorService = vendorService;
		}
		[HttpGet("BecomeVendor")]
		public async Task<IActionResult> BecomeVendor()
		{
			return View();

		}
		[HttpPost("BecomeVendor")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> BecomeVendor(Core.ViewModels.Requests.VendorRequest request)
		{
			if (!ModelState.IsValid)
			{
				return View(request);
			}
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized();
			}
			var response = await vendorService.SubmitVendorRequestAsync(request, userId);
			if (!response.Success)
			{
				ModelState.AddModelError(string.Empty, response.Message);
				return View(request);
			}
			return RedirectToAction("Index", "Home");
		}
		[HttpGet("Dashboard")]
		public async Task<IActionResult> Dashboard()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Unauthorized();
			var vendorIdResponse = await vendorService.ReturnVendorIdFromUserId(userId);
			if (!vendorIdResponse.Success || vendorIdResponse.Data == null)
				return NotFound("Vendor profile not found");

			var vendor = await vendorService.GetVendorByIdAsync(vendorIdResponse.Data as string, includeBooks: true);
			if (vendor == null)
				return NotFound("Vendor not found");

			return View(vendor);
		}


	}
}
