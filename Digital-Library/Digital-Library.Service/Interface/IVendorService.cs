using Digital_Library.Core.Enums;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
namespace Digital_Library.Service.Interface;
public interface IVendorService
{

	Task<Response> SubmitVendorRequestAsync(VendorRequest request, string userId);


	Task<Vendor?> GetVendorByIdAsync(string vendorId, bool includeBooks = false);


	Task<IEnumerable<Vendor>> GetVendorsAsync(VendorStatus? status = null);


	Task<Response> UpdateVendorProfileAsync(string vendorId, VendorUpdateRequest request);


	Task<Response> ChangeStatusAsync(string vendorId, VendorStatus status, string? reason = null);
}



