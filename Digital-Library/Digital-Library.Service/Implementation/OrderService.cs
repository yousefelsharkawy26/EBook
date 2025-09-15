using Digital_Library.Core.Constant;
using Digital_Library.Core.Enum;
using Digital_Library.Core.Enums;
using Digital_Library.Core.Models;
using Digital_Library.Core.Services;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Helpers;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace Digital_Library.Service.Services
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<OrderService> _logger;
		private readonly UserPdfEncryptionService _userpdfEncryptionHelper;
		private readonly VendorPdfEncryption vendorPdfEncryption;
		private readonly IFileService fileService;

		public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger, 
			UserPdfEncryptionService pdfEncryptionHelper,
			VendorPdfEncryption vendorPdfEncryption,
			IFileService fileService)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
			_userpdfEncryptionHelper = pdfEncryptionHelper;
			this.vendorPdfEncryption = vendorPdfEncryption;
			this.fileService = fileService;
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
				foreach (var vendorGroup in items.GroupBy(i => i.VendorId))
				{
					var pdfItems = vendorGroup.Where(i => i.FormatType == FormatType.PDF || i.FormatType == FormatType.Borrowing).ToList();
					var physicalItems = vendorGroup.Where(i => i.FormatType == FormatType.Physical).ToList();

					if (pdfItems.Any())
					{
						var pdfOrderHeader = new OrderHeader
						{
							VendorId = vendorGroup.Key,
							OrderDetails = new List<OrderDetail>(),
							Status = Status.Complete
						};
						decimal pdfTotal = 0;

						foreach (var item in pdfItems)
						{
							if (item.Quantity <= 0 || item.Price < 0)
								return Response.Fail("Invalid item quantity or price.");

							var book = await _unitOfWork.Books.GetSingleAsync(b => b.Id == item.BookId);
							if (book == null)
								return Response.Fail($"Book not found: {item.BookId}");

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
								case FormatType.Borrowing:
									orderDetail.Price = book.PricePDFPerDay ?? 0;
									break;
							}

							pdfTotal += orderDetail.Price * orderDetail.Quantity;
							pdfOrderHeader.OrderDetails.Add(orderDetail);
						}

						pdfOrderHeader.TotalAmount = pdfTotal;
						order.OrderHeaders.Add(pdfOrderHeader);
					}
					if (physicalItems.Any())
					{
						var physicalOrderHeader = new OrderHeader
						{
							VendorId = vendorGroup.Key,
							OrderDetails = new List<OrderDetail>()
						};
						decimal physicalTotal = 0;

						foreach (var item in physicalItems)
						{
							if (item.Quantity <= 0 || item.Price < 0)
								return Response.Fail("Invalid item quantity.");

							var book = await _unitOfWork.Books.GetSingleAsync(b => b.Id == item.BookId);
							if (book == null)
								return Response.Fail($"Book not found");

							if (book.Stock < item.Quantity)
								return Response.Fail($"Not enough stock for {book.Title}. Available: {book.Stock}");

							book.Stock -= item.Quantity;
							_unitOfWork.Books.Update(book);

							var orderDetail = new OrderDetail
							{
								BookId = item.BookId,
								Quantity = item.Quantity,
								FormatType = FormatType.Physical,
								Price = book.PricePhysical
							};

							physicalTotal += orderDetail.Price * orderDetail.Quantity;
							physicalOrderHeader.OrderDetails.Add(orderDetail);
						}

						physicalOrderHeader.TotalAmount = physicalTotal;
						order.OrderHeaders.Add(physicalOrderHeader);
					}
				}

				order.TotalAmount = order.OrderHeaders.Sum(h => h.TotalAmount);

				await _unitOfWork.Orders.AddAsync(order);
				await _unitOfWork.SaveChangesAsync();

				foreach (var header in order.OrderHeaders)
				{
					var transactionSuccess = await MakeTransaction(header.Id, header.TotalAmount);

					if (!transactionSuccess) continue;

					foreach (var detail in header.OrderDetails)
					{
						switch (detail.FormatType)
						{
							case FormatType.PDF:
								await AddPdfOrBorrowedBookForUser(userId, detail.BookId);
								break;

							case FormatType.Borrowing:
								int days = detail.Quantity; 
								await AddPdfOrBorrowedBookForUser(userId, detail.BookId, days);
								break;
						}
					}
				}


				await _unitOfWork.Commit();
				_logger.LogInformation("Order {OrderId} created successfully.", order.Id);

				return Response.Ok("Order created successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating order.");
				await _unitOfWork.RolleBack();
				return Response.Fail("Error creating order.");
			}
		}

		private async Task<bool> MakeTransaction(string orderHeaderId, decimal amount)
		{
			try
			{
				var orderHeader = await _unitOfWork.OrderHeaders.GetSingleAsync(oh => oh.Id == orderHeaderId);
				var vendor = await _unitOfWork.Vendors.GetSingleAsync(v => v.Id == orderHeader.VendorId);

				vendor.WalletBalance += amount;

				var transaction = new Transaction
				{
					OrderHeaderId = orderHeaderId,
					TransactionStatus = Status.Complete,
					Amount = amount
				};

				await _unitOfWork.Transactions.AddAsync(transaction);
				await _unitOfWork.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "MakeTransaction: Error while creating transaction.");
				return false;
			}
		}

		private async Task<bool> AddPdfOrBorrowedBookForUser(string userId, string bookId, int? days = null)
		{

			var user = await _unitOfWork.Users.GetSingleAsync(u => u.Id == userId);
			if (user == null) return false;

			var existing = await _unitOfWork.UserBookAccesses
							.GetSingleAsync(uba => uba.BookId == bookId && uba.UserId == userId && (uba.DueDate == null || uba.DueDate > DateTime.UtcNow));

			if (existing != null)
			{
				if (days.HasValue && existing.DueDate.HasValue)
				{
					existing.DueDate = existing.DueDate.Value.AddDays(days.Value);
					_unitOfWork.UserBookAccesses.Update(existing);
					await _unitOfWork.SaveChangesAsync();
				}
				return false;
			}
			var book = await _unitOfWork.Books.GetSingleAsync(b => b.Id == bookId);
			if (book == null) return false;

			var userBookAccess = new UserBookAccess
			{
				UserId = userId,
				BookId = bookId,
				AssignedDate = DateTime.UtcNow,
				BorrowDate = days.HasValue ? DateTime.UtcNow : null,
				DueDate = days.HasValue ? DateTime.UtcNow.AddDays(days.Value) : null
			};

			await _unitOfWork.UserBookAccesses.AddAsync(userBookAccess);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}

		public async Task<(string EncryptedFilePath, byte[] EncryptedDEK, byte[] IV, byte[] Tag)>
				DecryptAndEncryptForUserAsync(string bookFilePath, byte[] bookIV, byte[] bookTag, string userPublicKey, string outputFolder)
		{
			byte[] userPublicKeyBytes = Convert.FromBase64String(userPublicKey);

			var tempDecryptedPath = Path.Combine(outputFolder, $"temp_{Guid.NewGuid()}.pdf");


			await vendorPdfEncryption.DecryptFileToDiskAsync(bookFilePath, tempDecryptedPath, bookIV, bookTag);


			var userEncryptedPath = Path.Combine(outputFolder, $"{Guid.NewGuid()}.enc");


			using var rsa = RSA.Create();
			rsa.ImportSubjectPublicKeyInfo(userPublicKeyBytes, out _);


			var encryptionResult = await _userpdfEncryptionHelper.EncryptFileAsync(
							new FormFile(File.OpenRead(tempDecryptedPath), 0, new FileInfo(tempDecryptedPath).Length, "PDF", Path.GetFileName(tempDecryptedPath)),
							rsa,
							userEncryptedPath
			);

			File.Delete(tempDecryptedPath);

			return (userEncryptedPath, encryptionResult.EncryptedDEK, encryptionResult.IV, encryptionResult.Tag);
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
							oh => oh.Order != null && oh.Order.UserId == userId,
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
							oh => oh.VendorId == vendorId,
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
			var orderHeader = await _unitOfWork.OrderHeaders.GetSingleAsync(oh => oh.Id == orderHeaderId);

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
