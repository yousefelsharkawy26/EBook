using Digital_Library.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Digital_Library.Infrastructure.Context
{
    public class EBookContext: IdentityDbContext
    {
        public EBookContext(DbContextOptions options)
            : base(options)
        {
            
        }
        public DbSet<Vendor> Vendors { get; set; }


    }
}
