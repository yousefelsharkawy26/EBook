using Digital_Library.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Digital_Library.Infrastructure.Context
{
    public class EBookContext: IdentityDbContext
    {
        public EBookContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartsDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
    }
}
