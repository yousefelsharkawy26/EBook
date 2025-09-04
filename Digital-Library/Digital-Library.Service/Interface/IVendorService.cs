using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Responses;
using Microsoft.Identity.Client;

namespace Digital_Library.Service.Interface;
public interface IVendorService
{
    Task<IEnumerable<Vendor>> GetAllVendorsAsync();
    Task<Vendor> GetVendorByIdAsync(string id);

    //get vendor by user id instead of guid
    Task<Vendor> GetVendorByUserIdAsync(string userId);

    Task<Vendor> AddVendorAsync(Vendor vendor);
    Task<Vendor> UpdateVendorAsync(string id, Vendor vendor);
    Task<bool> DeleteVendorAsync(string id);
    Task<VendorDashboardResponse> GetVendorDashboardAsync(string vendorId);

}

