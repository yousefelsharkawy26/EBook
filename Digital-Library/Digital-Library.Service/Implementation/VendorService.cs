using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.Context;
using Digital_Library.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace Digital_Library.Service.Implementation;

    public class VendorService : IVendorService
    {
        private readonly EBookContext _context;

        public VendorService(EBookContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vendor>> GetAllVendorsAsync()
        {   
            return await _context.Vendors.ToListAsync();
        }

        public async Task<Vendor> GetVendorByIdAsync(string id)
        {
            return await _context.Vendors.FindAsync(id);
        }


    public async Task<Vendor> GetVendorByUserIdAsync(string userId)
    {
        return await _context.Vendors
            .Include(v => v.Books)
            .FirstOrDefaultAsync(v => v.UserId == userId);
    }


    public async Task<Vendor> AddVendorAsync(Vendor vendor)
        {
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();
            return vendor;
        }

        public async Task<Vendor> UpdateVendorAsync(string id, Vendor vendor)
        {
            //var existingVendor = await _context.Vendors.FindAsync(id);
            //if (existingVendor == null) return null;

            //existingVendor.LibraryName = vendor.LibraryName;
            //existingVendor.Address = vendor.Address;
            //existingVendor.City = vendor.City;
            //existingVendor.State = vendor.State;
            //existingVendor.ZipCode = vendor.ZipCode;
            //existingVendor.ContactNumber = vendor.ContactNumber;
            //existingVendor.IdentityImagesUrl = vendor.IdentityImagesUrl;
            //existingVendor.WalletBalance = vendor.WalletBalance;

            //await _context.SaveChangesAsync();
            //return existingVendor;
            return null;
    }

    public async Task<bool> DeleteVendorAsync(string id)
    {
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor == null) return false;

        _context.Vendors.Remove(vendor);
        await _context.SaveChangesAsync();
        return true;
    }


    //VendorDashboard
    public async Task<VendorDashboardResponse> GetVendorDashboardAsync(string vendorId)
    {
        var vendor = await _context.Vendors
            .Include(v => v.Books)
                .ThenInclude(b => b.OrderDetails)
            .FirstOrDefaultAsync(v => v.Id == vendorId);

        if (vendor == null) return null;

        var totalSales = vendor.Books?
            .SelectMany(b => b.OrderDetails ?? new List<OrderDetail>())
            .Sum(od => od.Quantity * od.Price) ?? 0;

        var totalBooksSold = vendor.Books?
            .SelectMany(b => b.OrderDetails ?? new List<OrderDetail>())
            .Sum(od => od.Quantity) ?? 0;

        return new VendorDashboardResponse
        {
            VendorId = vendor.Id,
            LibraryName = vendor.LibraryName,
            WalletBalance = vendor.WalletBalance,
            TotalBooks = vendor.Books?.Count ?? 0,
            TotalBooksSold = totalBooksSold,
            TotalRevenue = totalSales
        };
    }

}

