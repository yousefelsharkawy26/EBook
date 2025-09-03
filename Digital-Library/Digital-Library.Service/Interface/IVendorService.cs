using Digital_Library.Core.Models;

namespace Digital_Library.Service.Interface;
public interface IVendorService
{
    Task<IEnumerable<Vendor>> GetAllVendorsAsync();
    Task<Vendor> GetVendorByIdAsync(int id);
    Task<Vendor> AddVendorAsync(Vendor vendor);
    Task<Vendor> UpdateVendorAsync(int id, Vendor vendor);
    Task<bool> DeleteVendorAsync(int id);
}

