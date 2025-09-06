using Digital_Library.Core.Enum;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Digital_Library.Controllers
{
	[Route("Orders")]
	[Authorize]
	public class OrdersController : Controller
	{
		private readonly IOrderService _orderService;
		private readonly ICartService cartService;

		public OrdersController(IOrderService orderService,ICartService cartService)
		{
			_orderService = orderService;
			this.cartService = cartService;
		}
		[HttpGet("MyOrders")]
		public async Task<IActionResult> MyOrders()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Unauthorized();
			var orders = await _orderService.GetUserOrders(userId);
			return View(orders);
		}
		[HttpPost("PlaceOrder")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> PlaceOrder(PlaceOrderRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Unauthorized();
			var cart = await cartService.GetCartAsync(userId);
			var items=((Cart)cart.Data)?.CartDetails.Select(cd => new OrderDetailRequest
			{
				BookId = cd.BookId,
				Quantity = cd.Quantity,
				Price = cd.FormatType is Core.Enums.FormatType.Physical?cd.Book.PricePhysical:(cd.Book.PricePdf??0),
				VendorId = cd.Book.VendorId
			}).ToList();
			if (!cart.Success || cart.Data == null || cart.Data.CartDetails == null || !cart.Data.CartDetails.Any())
			{
				ModelState.AddModelError(string.Empty, "Your cart is empty.");
				return View("Index", cart.Data); 
			}


		}


	}
}
