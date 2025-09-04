using Digital_Library.Core.Constant;
using Digital_Library.Core.Enum;
using Digital_Library.Core.Enums;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Digital_Library.Service.Services
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<OrderService> _logger;

		public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task<Response> CreateOrderAsync(string userId, List<OrderDetailRequest> items)
		{
			try
			{
				if (items == null || !items.Any())
					return Response.Fail("Order must contain at least one book.");

				var order = new Order
				{
					UserId = userId,
					OrderDate = DateTime.UtcNow,
					Status = Status.Pending,
					TotalAmount = items.Sum(i => i.Price * i.Quantity),
					OrderDetails = new List<OrderDetail>()
				};

				foreach (var item in items)
				{
					var book = await _unitOfWork.Books.GetByIdAsync(item.BookId);
					if (book == null)
						return Response.Fail($"Book with Id {item.BookId} not found.");

					order.OrderDetails.Add(new OrderDetail
					{
						BookId = item.BookId,
						FormatType = item.FormatType,
						Price = item.Price,
						Quantity = item.Quantity
					});
				}

				await _unitOfWork.Orders.AddAsync(order);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Order {OrderId} created for user {UserId}", order.Id, userId);
				return Response.Ok("Order created successfully.", order);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating order for user {UserId}", userId);
				return Response.Fail("An error occurred while creating the order.");
			}
		}

		public async Task<IEnumerable<Order>> GetUserOrdersAsync(string? userId = null)
		{
			IQueryable<Order> query;

			if (!string.IsNullOrEmpty(userId))
			{
				query = _unitOfWork.Orders.GetManyQuery(
								predicate: o => o.UserId == userId,
								includes: new Expression<Func<Order, object>>[]
								{
																o => o.OrderDetails,
																o => o.OrderDetails.Select(d => d.Book)
								});
			}
			else
			{
				query = _unitOfWork.Orders.GetAllQuery(
								includes: new Expression<Func<Order, object>>[]
								{
																o => o.OrderDetails,
																o => o.OrderDetails.Select(d => d.Book)
								});
			}

			return await query.ToListAsync();
		}

		public async Task<Response> GetOrderByIdAsync(string orderId)
		{
			var order = await _unitOfWork.Orders.GetSingleAsync(
							o => o.Id == orderId,
							o => o.OrderDetails,
							o => o.OrderDetails.Select(d => d.Book),
							o => o.User
			);

			if (order == null)
				return Response.Fail("Order not found.");

			return Response.Ok("Order retrieved successfully.", order);
		}

		public async Task<Response> UpdateOrderStatusAsync(string orderId, Status status)
		{
			try
			{
				var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
				if (order == null)
					return Response.Fail("Order not found.");

				order.Status = status;
				_unitOfWork.Orders.Update(order);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Order {OrderId} status updated to {Status}", orderId, status);
				return Response.Ok("Order status updated successfully.", order);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating status for order {OrderId}", orderId);
				return Response.Fail("An error occurred while updating order status.");
			}
		}

	}
}
