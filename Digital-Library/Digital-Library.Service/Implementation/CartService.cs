using Digital_Library.Core.Enums;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Digital_Library.Service.Implementation
{
	public class CartService : ICartService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<CartService> _logger;

		public CartService(IUnitOfWork unitOfWork, ILogger<CartService> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task<Response> GetCartAsync(string userId)
		{
			var cart = await _unitOfWork.Carts
							.GetSingleWithIncludeAsync(
											c => c.UserId == userId,
											q => q.Include(c => c.CartDetails).ThenInclude(cd => cd.Book)
							);

			if (cart == null)
			{
				cart = new Cart { UserId = userId, CartDetails = new List<CartDetail>() };
				await _unitOfWork.Carts.AddAsync(cart);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("New cart created for user {UserId}", userId);
				return Response.Ok("New cart created", cart);
			}

			_logger.LogInformation("Retrieved cart with {Count} items for user {UserId}",
							cart.CartDetails?.Count ?? 0, userId);

			return Response.Ok("Cart retrieved successfully", cart);
		}

		public async Task<Response> AddItemAsync(string userId, CartDetailRequest request)
		{
			var cartResponse = await GetCartAsync(userId);
			var cart = (Cart)cartResponse.Data!;

			var existingItem = cart.CartDetails
							.FirstOrDefault(cd => cd.BookId == request.BookId && cd.FormatType == request.FormatType);

			if (existingItem != null)
			{
				existingItem.Quantity += request.Quantity;
				_unitOfWork.CartDetails.Update(existingItem);

				_logger.LogInformation("Updated quantity of book {BookId} in cart for user {UserId}",
								request.BookId, userId);
			}
			else
			{
				var newItem = new CartDetail
				{
					BookId = request.BookId,
					FormatType = request.FormatType,
					Quantity = request.Quantity,
					CartId = cart.Id
				};

				await _unitOfWork.CartDetails.AddAsync(newItem);

				_logger.LogInformation("Added book {BookId} to cart for user {UserId}",
								request.BookId, userId);
			}

			await _unitOfWork.SaveChangesAsync();
			return Response.Ok("Item added/updated in cart", cart);
		}

		public async Task<Response> RemoveItemAsync(string userId, string bookId, FormatType formatType)
		{
			var cartResponse = await GetCartAsync(userId);
			var cart = (Cart)cartResponse.Data!;

			var item = cart.CartDetails
							.FirstOrDefault(cd => cd.BookId == bookId && cd.FormatType == formatType);

			if (item != null)
			{
				_unitOfWork.CartDetails.Delete(item);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Removed book {BookId} from cart for user {UserId}", bookId, userId);
				return Response.Ok("Item removed from cart", cart);
			}

			return Response.Fail("Item not found in cart");
		}

		public async Task<Response> UpdateCartAsync(string userId, string bookId, int quantity, FormatType formatType)
		{
			var cartResponse = await GetCartAsync(userId);
			var cart = (Cart)cartResponse.Data!;

			var item = cart.CartDetails
							.FirstOrDefault(cd => cd.BookId == bookId && cd.FormatType == formatType);

			if (item != null)
			{
				item.Quantity = quantity;
				_unitOfWork.CartDetails.Update(item);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Updated quantity of book {BookId} to {Quantity} in cart for user {UserId}",
								bookId, quantity, userId);

				return Response.Ok("Item quantity updated", cart);
			}

			return Response.Fail("Item not found in cart");
		}

		public async Task<Response> ClearCartAsync(string userId)
		{
			var cartResponse = await GetCartAsync(userId);
			var cart = (Cart)cartResponse.Data!;

			if (cart.CartDetails.Any())
			{
				_unitOfWork.CartDetails.DeleteRange(cart.CartDetails);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Cleared cart for user {UserId}", userId);
				return Response.Ok("Cart cleared", cart);
			}

			return Response.Fail("Cart is already empty");
		}
	}
}
