using Digital_Library.Core.ViewModels;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;

namespace Digital_Library.Service.Interface
{
	public interface IUserService
	{
		Task<Response> UpdateProfileAsync(string userId, UserRequest request);
		Task<Response> GetProfileAsync(string userId);
        Task<IEnumerable<CustomerSummaryViewModel>> GetAllCustomersAsync();
        Task<CustomerDetailsViewModel?> GetCustomerDetailsAsync(string customerId);
        Task<bool> ToggleUserStatusAsync(string customerId);
    }
}
