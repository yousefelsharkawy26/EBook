using Digital_Library.Core.Enum;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;

namespace Digital_Library.Service.Interface;

public interface IOrderService
{
	Task<Response> CreateOrderAsync(string userId, List<OrderDetailRequest> items, string address, string phoneNumber);

Task<	IQueryable<OrderHeader>> GetVendorOrders(string vendorId);
	Task<IQueryable<OrderHeader>> GetUserOrders(string UserId);

	Task<Response> GetOrderHeaderDetailsByIdAsync(string orderHeaderId);

	Task<Response> UpdateOrderStatusAsync(string orderHeaderId, Status status);

}
