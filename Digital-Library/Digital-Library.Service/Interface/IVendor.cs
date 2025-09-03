using Digital_Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Digital_Library.Infrastructure.Context;



namespace Digital_Library.Service.Interface
{

    public interface IVendorService
    {
        Task<IEnumerable<Vendor>> GetAllVendorsAsync();
        Task<Vendor> GetVendorByIdAsync(int id);
        Task<Vendor> AddVendorAsync(Vendor vendor);
        Task<Vendor> UpdateVendorAsync(int id, Vendor vendor);
        Task<bool> DeleteVendorAsync(int id);
    }

    
}