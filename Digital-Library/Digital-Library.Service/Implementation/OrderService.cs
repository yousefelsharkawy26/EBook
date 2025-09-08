using Digital_Library.Core.Constant;
using Digital_Library.Core.Enum;
using Digital_Library.Core.Enums;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Components.Forms;
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

			await _unitOfWork.BeginTransactionAsync();

			var order = new Order
			{
				UserId = userId,
				OrderHeaders = new List<OrderHeader>(),
				Address = request.Address,
				PhoneNumber = request.PhoneNumber
			};

			try
			{
				foreach (var vendorGroup in items.GroupBy(s => s.VendorId))
				{
					var orderHeader = new OrderHeader
					{
						VendorId = vendorGroup.Key,
						OrderDetails = new List<OrderDetail>()
					};

					decimal orderHeaderAmount = 0;

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
							return Response.Fail($"Book  not found.");
						}

						if (book.Stock < item.Quantity)
						{
							return Response.Fail($"Not enough stock for {book.Title}. Available: {book.Stock}");
						}

						book.Stock -= item.Quantity;
						_unitOfWork.Books.Update(book);

						var orderDetail = new OrderDetail
						{
							BookId = item.BookId,
							Quantity = item.Quantity,
							FormatType = item.FormatType
						};

						switch (item.FormatType)
						{
							case FormatType.PDF:
								orderDetail.Price = book.PricePdf ?? 0;
								break;
							case FormatType.Physical:
								orderDetail.Price = book.PricePhysical;
								break;
							case FormatType.Borrowing:
								orderDetail.Price = book.PricePDFPerDay ?? 0;
								break;
							default:
								orderDetail.Price = 0;
								break;
						}

						orderHeader.OrderDetails.Add(orderDetail);
						orderHeaderAmount += orderDetail.Price * orderDetail.Quantity;

						if (orderDetail.FormatType == FormatType.PDF)
						{
						var res=	await AddPdfBooksToUser(userId, book.Id);
							if (!res)
							{
								await _unitOfWork.RolleBack();
								return Response.Fail($"Already Own Book{book.Title}");
							}
						}
						else if (orderDetail.FormatType == FormatType.Borrowing)
						{
							await AddBorrowPdfBooksToUser(userId, book.Id, item.Quantity);
						}

					}

					orderHeader.TotalAmount = orderHeaderAmount;
					order.OrderHeaders.Add(orderHeader);
				}

				order.TotalAmount = order.OrderHeaders.Sum(h => h.TotalAmount);

				await _unitOfWork.Orders.AddAsync(order);
				await _unitOfWork.SaveChangesAsync();

				foreach (var header in order.OrderHeaders)
				{
					await MakeTransaction(header.Id, header.TotalAmount);
				}

				await _unitOfWork.Commit();

				_logger.LogInformation("CreateOrderAsync: Order {OrderId} created successfully.", order.Id);
				return Response.Ok("Order created successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "CreateOrderAsync: Error creating order.");
				await _unitOfWork.RolleBack();
				return Response.Fail("Error creating order.");
			}
		}

		private async Task<bool> MakeTransaction(string ordeHeaderId, decimal amount)
		{
			var transaction = new Transaction
			{
				OrderHeaderId = ordeHeaderId,
				TransactionStatus = Status.Complete,
				Amount = amount
			};

			try
			{
				await _unitOfWork.Transactions.AddAsync(transaction);

				var orderHeader = await _unitOfWork.OrderHeaders.GetSingleAsync(t => t.Id == ordeHeaderId);
				var vendor = await _unitOfWork.Vendors.GetSingleAsync(v => v.Id == orderHeader.VendorId);

				vendor.WalletBalance += amount;

				await _unitOfWork.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "MakeTransaction: Error while creating transaction.");
				return false;
			}
		}

		private async Task<bool> AddPdfBooksToUser(string userId, string bookId)
		{
			var res=await _unitOfWork.UserPdfBooks.GetSingleAsync(upb => upb.BookId == bookId && upb.UserId == userId);
			if (res != null)
			{
				return false;
			}
			var userPdfBook = new UserPdfBook
			{
				BookId = bookId,
				UserId = userId
			};

			try
			{
				await _unitOfWork.UserPdfBooks.AddAsync(userPdfBook);
				await _unitOfWork.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "AddPdfBooksToUser: Error while adding book {BookId} to user {UserId}.", bookId, userId);
				return	false;
			}
		}

		private async Task<bool> AddBorrowPdfBooksToUser(string userId, string bookId,int days)
		{
			var res = await _unitOfWork.Borrowings.GetSingleAsync(b => b.BookId == bookId && b.UserId == userId && b.DueDate>DateTime.UtcNow);
			if (res != null)
			{
				res.DueDate= res.DueDate.AddDays(days);
				_unitOfWork.Borrowings.Update(res);
				await _unitOfWork.SaveChangesAsync();
				return true;
			}
			var borrowpdf = new Borrowing
			{
				BookId = bookId,
				UserId = userId,
				DueDate=	DateTime.UtcNow.AddDays(days)
			};
			try
			{
				await _unitOfWork.Borrowings.AddAsync(borrowpdf);
				await _unitOfWork.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "AddBorrowPdfBooksToUser: Error while adding book {BookId} to user {UserId}.", bookId, userId);
				return	false;
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

		public async Task<IQueryable<OrderHeader>> GetUserOrders(string userId)
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
												oh => oh.Vendor.User,
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
												oh => oh.Order.User,
												oh => oh.Vendor,
												oh => oh.Vendor.User,
												oh => oh.OrderDetails
							},
							thenIncludes: new Func<IQueryable<OrderHeader>, IIncludableQueryable<OrderHeader, object>>[]
							{
												q => q.Include(oh => oh.OrderDetails).ThenInclude(od => od.Book)
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


	}
}
