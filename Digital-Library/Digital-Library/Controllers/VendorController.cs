using Digital_Library.Core.Constant;
using Digital_Library.Core.Enum;
using Digital_Library.Core.Models;
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
		private readonly IOrderService orderService;

		public VendorController(IVendorService vendorService,IOrderService orderService)
		{
			this.vendorService = vendorService;
			this.orderService = orderService;
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
		[Authorize(Roles = Roles.Vendor)]
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

		[HttpGet("MyLibraryOrders")]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> MyLibraryOrders()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Forbid();
			var vendorIdResponse = await vendorService.ReturnVendorIdFromUserId(userId);
			if (!vendorIdResponse.Success || vendorIdResponse.Data == null)
				return NotFound("Vendor profile not found");
			var orders = await orderService.GetVendorOrders(vendorIdResponse.Data as string);

			return View(orders); 
		}
		[HttpPost("ChangeStatus")]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> ChangeStatus(string orderHeaderId, Status status)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Forbid();

			var vendorIdResponse = await vendorService.ReturnVendorIdFromUserId(userId);
			if (!vendorIdResponse.Success || vendorIdResponse.Data == null)
				return NotFound("Vendor profile not found");

			var vendorId = vendorIdResponse.Data as string;

			var orderHeaderResponse = await orderService.GetOrderHeaderDetailsByIdAsync(orderHeaderId);
			if (!orderHeaderResponse.Success || orderHeaderResponse.Data == null)
				return NotFound("Order not found");

			var orderHeader = orderHeaderResponse.Data as OrderHeader;
			if (orderHeader == null || orderHeader.VendorId != vendorId)
				return Forbid("You are not authorized to change this order's status.");

			var updateResponse = await orderService.UpdateOrderStatusAsync(orderHeaderId, status);
			if (!updateResponse.Success)
			{
				TempData["ErrorMessage"] = updateResponse.Message ?? "Failed to update order status.";
				return RedirectToAction(nameof(MyLibraryOrders));
			}

			TempData["SuccessMessage"] = "Order status updated successfully.";
			return RedirectToAction(nameof(MyLibraryOrders));
		}
	}
}
