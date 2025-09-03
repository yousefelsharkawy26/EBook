using Digital_Library.Core.Models;
using Digital_Library.Infrastructure.Context;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Implementation
{
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

        public async Task<Vendor> GetVendorByIdAsync(int id)
        {
            return await _context.Vendors.FindAsync(id);
        }

        public async Task<Vendor> AddVendorAsync(Vendor vendor)
        {
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();
            return vendor;
        }

        public async Task<Vendor> UpdateVendorAsync(int id, Vendor vendor)
        {
            var existingVendor = await _context.Vendors.FindAsync(id);
            if (existingVendor == null) return null;

            existingVendor.LibraryName = vendor.LibraryName;
            existingVendor.Address = vendor.Address;
            existingVendor.City = vendor.City;
            existingVendor.State = vendor.State;
            existingVendor.ZipCode = vendor.ZipCode;
            existingVendor.ContactNumber = vendor.ContactNumber;
            existingVendor.IdentityImagesUrl = vendor.IdentityImagesUrl;
            existingVendor.WalletBalance = vendor.WalletBalance;

            await _context.SaveChangesAsync();
            return existingVendor;
        }

        public async Task<bool> DeleteVendorAsync(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null) return false;

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
