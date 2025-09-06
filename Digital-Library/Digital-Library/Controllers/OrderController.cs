using Digital_Library.Core.Enum;
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

		public OrdersController(IOrderService orderService)
		{
			_orderService = orderService;
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
			var items=await	_orderService.GetUserOrders(userId);
			if (items == null || !items.Any())
				return BadRequest("Order details cannot be empty.");
			var result = await _orderService.CreateOrderAsync(userId, request.Items,request.Address,request.PhoneNumber);
			if (result.Success)
				return Ok(result);
			else
				return BadRequest(result.Message);
		}


	}
}
