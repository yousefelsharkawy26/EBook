using Digital_Library.Core.Constant;
using Digital_Library.Core.ViewModels.Requests;
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

		[HttpGet("EditLibraryProfile")]
		[Authorize(Roles =Roles.Vendor)]
		public async Task<IActionResult> EditLibraryProfile()

		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Forbid();
			var vendorIdResponse = await vendorService.ReturnVendorIdFromUserId(userId);
			if (!vendorIdResponse.Success || vendorIdResponse.Data == null)
				return NotFound("Vendor profile not found");
			var vendor = await vendorService.GetVendorByIdAsync(vendorIdResponse.Data as string);
			var model = new VendorUpdateRequest
			{
				LibraryName = vendor.LibraryName,
				City = vendor.City,
				State = vendor.State,
				ZipCode = vendor.ZipCode,
				ContactNumber = vendor.ContactNumber
			};
			ViewBag.ExistingLogoPath = vendor.LibraryLogoUrl ;
			return View(model);
		}
		[HttpPost("EditLibraryProfile")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> EditLibraryProfile(VendorUpdateRequest request)
		{
			if (!ModelState.IsValid)
			{
				return View(request);
			}
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Forbid();
			var vendorIdResponse = await vendorService.ReturnVendorIdFromUserId(userId);
			if (!vendorIdResponse.Success || vendorIdResponse.Data == null)
				return NotFound("Vendor profile not found");
			var response = await vendorService.UpdateVendorProfileAsync(vendorIdResponse.Data as string, request);
			if (!response.Success)
			{
				ModelState.AddModelError(string.Empty, response.Message);
				return View(request);
			}
			return RedirectToAction(nameof(Dashboard));
		}



	}
}
