using Digital_Library.Core.Enum;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
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
		private readonly IUnitOfWork unitOfWork;

		public OrdersController(
			IOrderService orderService,
			ICartService cartService,
			IUnitOfWork unitOfWork)
		{
			_orderService = orderService;
			this.cartService = cartService;
			this.unitOfWork = unitOfWork;
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
		public async Task<IActionResult> PlaceOrder([FromForm]PlaceOrderRequest request)
		{
			var userId = GetUserId();
			if (userId == null)
				return Unauthorized();

			var cart = await GetUserCart(userId);
			if (cart == null)
				return View("Index", null);

			var validationResult = ValidateCart(cart, userId);
			if (validationResult != null)
				return validationResult;


			var items = await BuildOrderItems(cart, userId);


			var response = await _orderService.CreateOrderAsync(userId, items, request);
			if (!response.Success)
			{
				ModelState.AddModelError(string.Empty, response.Message);
				return View("Index", cart);
			}

			await cartService.ClearCartAsync(userId);

			TempData["Success"] = "Your order has been placed successfully!";
			return RedirectToAction("MyOrders");
		}

		private string? GetUserId()
		{
			return User.FindFirstValue(ClaimTypes.NameIdentifier);
		}

		private async Task<Cart?> GetUserCart(string userId)
		{
			var userCart = await cartService.GetCartAsync(userId);
			var cart = userCart.Data as Cart;

			if (!userCart.Success || cart?.CartDetails == null || !cart.CartDetails.Any())
			{
				ModelState.AddModelError(string.Empty, "Your cart is empty.");
				return null;
			}

			return cart;
		}

		private IActionResult? ValidateCart(Cart cart, string userId)
		{
			foreach (var cd in cart.CartDetails)
			{
				if (cd.Book?.Vendor.UserId == userId)
				{
					TempData["Error"] = $"You cannot order your own book: {cd.Book.Title}";
					return RedirectToAction("Index", "Cart");
				}
			}
			var groupedBooks = cart.CartDetails.GroupBy(cd => cd.BookId);
			foreach (var group in groupedBooks)
			{
				var hasPdf = group.Any(cd => cd.FormatType == Core.Enums.FormatType.PDF);
				var hasBorrowing = group.Any(cd => cd.FormatType == Core.Enums.FormatType.Borrowing);

				if (hasPdf && hasBorrowing)
				{
					ModelState.AddModelError(string.Empty,
									$"You cannot order the same book '{group.First().Book.Title}' as both PDF and Borrowing.");
					return View("Index", cart);
				}
			}

			return null; 
		}

		private async Task<List<OrderDetailRequest>> BuildOrderItems(Cart cart, string userId)
		{
			var items = new List<OrderDetailRequest>();

			foreach (var cd in cart.CartDetails)
			{
				if (cd.Book == null) continue;

				decimal price = 0;

				switch (cd.FormatType)
				{
					case Core.Enums.FormatType.Physical:
						price = cd.Book.PricePhysical * cd.Quantity;
						break;

					case Core.Enums.FormatType.PDF:
						price = (cd.Book.PricePdf ?? 0) * 1;
						break;

					case Core.Enums.FormatType.Borrowing:
						price = (cd.Book.PricePDFPerDay ?? 0) * 1;

						var borrowing = new Borrowing
						{
							BookId = cd.BookId,
							UserId = userId,
							BorrowDate = DateTime.UtcNow,
							DueDate = DateTime.UtcNow.AddDays(cd.Quantity)
						};
						await unitOfWork.Borrowings.AddAsync(borrowing);
						break;
				}

				items.Add(new OrderDetailRequest
				{
					BookId = cd.BookId,
					Quantity = cd.Quantity,
					Price = price,
					VendorId = cd.Book.VendorId,
					FormatType = cd.FormatType
				});
			}

			return items;
		}


	}


}

