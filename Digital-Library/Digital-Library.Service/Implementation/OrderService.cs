using Digital_Library.Core.Constant;
using Digital_Library.Core.Enum;
using Digital_Library.Core.Enums;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

		public async Task<Response> CreateOrderAsync(string userId, List<OrderDetailRequest> items, PlaceOrderRequest request)
		{
			if (string.IsNullOrEmpty(userId) || items == null || !items.Any())
			{
				_logger.LogWarning("CreateOrderAsync: Invalid userId or empty items list.");
				return Response.Fail("Invalid items list.");
			}

			var order = new Order
			{
				UserId = userId,
				TotalAmount = items.Sum(i => i.Price * i.Quantity),
				OrderHeaders = new List<OrderHeader>(),
				Address = request.Address,
				PhoneNumber = request.PhoneNumber
			};

			// نجمع الكتب حسب Vendor
			foreach (var vendorGroup in items.GroupBy(s => s.VendorId))
			{
				var orderHeader = new OrderHeader
				{
					OrderId = order.Id,
					VendorId = vendorGroup.Key,
					OrderDetails = new List<OrderDetail>()
				};

				foreach (var item in vendorGroup)
				{
					if (item.Quantity <= 0 || item.Price < 0)
					{
						_logger.LogWarning("CreateOrderAsync: Invalid item quantity or price.");
						return Response.Fail("Invalid item quantity or price.");
					}

					var book = await _unitOfWork.Books.GetSingleAsync(b => b.Id == item.BookId);
					if (book == null)
					{
						_logger.LogWarning("CreateOrderAsync: Book with ID {BookId} not found.", item.BookId);
						return Response.Fail($"Book with ID {item.BookId} not found.");
					}

					orderHeader.OrderDetails.Add(new OrderDetail
					{
						BookId = item.BookId,
						Quantity = item.Quantity,
						Price = item.Price
					});
				}

				order.OrderHeaders.Add(orderHeader);
			}

			try
			{
				await _unitOfWork.Orders.AddAsync(order);
				await _unitOfWork.SaveChangesAsync();
				_logger.LogInformation("CreateOrderAsync: Order {OrderId} created successfully.", order.Id);
				return Response.Ok("Order created successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "CreateOrderAsync: Error creating order.");
				return Response.Fail("Error creating order.");
			}
		}


		public async Task<Response> GetOrderHeaderDetailsByIdAsync(string orderHeaderId)
		{
			if (string.IsNullOrEmpty(orderHeaderId))
				return Response.Fail("OrderHeaderId is required");
			var orderHeader = await _unitOfWork.OrderHeaders.GetSingleWithIncludeAsync(
							oh => oh.Id.ToString() == orderHeaderId,
							q => q.Include(oh => oh.Order)
													.ThenInclude(o => o.User)
													.Include(oh => oh.Vendor)
													.Include(oh => oh.OrderDetails)
													.ThenInclude(od => od.Book)
			);

			if (orderHeader == null)
				return Response.Fail("OrderHeader not found");

			return Response.Ok("OrderHeader details retrieved successfully", orderHeader);
		}

		public async Task< IQueryable<OrderHeader>> GetUserOrders(string userId)
		{
			if (string.IsNullOrEmpty(userId))
				return Enumerable.Empty<OrderHeader>().AsQueryable();

			var query = _unitOfWork.OrderHeaders.GetManyQuery(
							predicate: oh => oh.Order != null && oh.Order.UserId == userId,
							includes: new Expression<Func<OrderHeader, object>>[]
							{
												oh => oh.Order,
												oh => oh.Order.User,
												oh => oh.Vendor,
												oh => oh.OrderDetails
							},
							thenIncludes: new Func<IQueryable<OrderHeader>, IIncludableQueryable<OrderHeader, object>>[]
							{
												q => q.Include(oh => oh.OrderDetails).ThenInclude(od => od.Book)
							}
			);

			return query;
		}

		public async Task<IQueryable<OrderHeader>> GetVendorOrders(string vendorId)
		{
			var query = _unitOfWork.OrderHeaders.GetManyQuery(
							predicate: oh => oh.VendorId == vendorId,
							includes: new Expression<Func<OrderHeader, object>>[]
							{
												oh => oh.Order,             
            oh => oh.Order.User        
							}
			);

			return query;
		}

		public async Task<Response> UpdateOrderStatusAsync(string orderHeaderId, Status status)
		{
			var orderHeader = await _unitOfWork.OrderHeaders
							.GetSingleAsync(oh => oh.Id == orderHeaderId);

			if (orderHeader == null)
				return Response.Fail("Order header not found");

			orderHeader.Status = status;

			_unitOfWork.OrderHeaders.Update(orderHeader);
			await _unitOfWork.SaveChangesAsync();

			_logger.LogInformation("Updated status of OrderHeader {OrderHeaderId} to {Status}", orderHeaderId, status);

			return Response.Ok("OrderHeader status updated successfully", orderHeader);
		}

		private async Task<bool> MakeTransation(string ordeHeaderId,decimal amount)
		{
			var transaction = new Transaction
			{
				OrderHeaderId = ordeHeaderId,
				TransactionDate = DateTime.UtcNow,
				TransactionStatus=Status.Complete
			};
			try
			{
				await _unitOfWork.Transactions.AddAsync(transaction);
				await _unitOfWork.SaveChangesAsync();
				return	true;
			}
			catch (Exception)
			{
				return	false;
			}
		}
	}
}
