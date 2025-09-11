using Digital_Library.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Digital_Library.Infrastructure.Context
{
	public class EBookContext : IdentityDbContext
	{
		public EBookContext(DbContextOptions options)
						: base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- PARENT-CHILD OWNERSHIP RULES (CASCADE) ---
            // These are relationships where the child cannot exist without the parent.

            // A User owns their Borrowing history.
            modelBuilder.Entity<Borrowing>()
                .HasOne(b => b.User)
                .WithMany(u => u.borrowings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // A Cart owns its details.
            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.Cart)
                .WithMany(c => c.CartDetails)
                .HasForeignKey(cd => cd.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // An OrderHeader owns its OrderDetails. THIS IS CRUCIAL.
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.OrderHeader)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderHeaderId)
                .OnDelete(DeleteBehavior.Cascade); // KEEP THIS AS CASCADE

            // A User owns their purchased PDF book licenses.
            modelBuilder.Entity<UserPdfBook>()
                .HasOne(upb => upb.User)
                .WithMany(u => u.userPdfBooks)
                .HasForeignKey(upb => upb.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // --- REFERENCE RELATIONSHIPS (RESTRICT TO BREAK CYCLES) ---
            // These relationships are for reference. Deleting the parent should not affect the child.

            // A Book cannot be deleted if it has borrowing history.
            modelBuilder.Entity<Borrowing>()
                .HasOne(b => b.Book)
                .WithMany(book => book.Borrowings)
                .HasForeignKey(b => b.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // A Book cannot be deleted if it's in someone's cart.
            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.Book)
                .WithMany(b => b.CartDetails)
                .HasForeignKey(cd => cd.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // A Book cannot be deleted if it has been sold (part of an order).
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Book)
                .WithMany(b => b.OrderDetails) // Assuming Book has ICollection<OrderDetail>
                .HasForeignKey(od => od.BookId)
                .OnDelete(DeleteBehavior.Restrict); // CHANGE TO RESTRICT

            // A Vendor cannot be deleted if they have historical orders.
            modelBuilder.Entity<OrderHeader>()
                .HasOne(oh => oh.Vendor)
                .WithMany(v => v.OrderHeaders)
                .HasForeignKey(oh => oh.VendorId)
                .OnDelete(DeleteBehavior.Restrict); // CHANGE TO RESTRICT

            // A Book cannot be deleted if users have purchased its PDF.
            modelBuilder.Entity<UserPdfBook>()
                .HasOne(upb => upb.Book)
                .WithMany(b => b.userPdfBooks)
                .HasForeignKey(upb => upb.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- OTHER CONFIGURATIONS ---
            modelBuilder.Entity<UserPdfBook>()
                .HasKey(b => new { b.UserId, b.BookId });
        }

        #region Entities
        public DbSet<Book> Books { get; set; }
		public DbSet<Borrowing> Borrowings { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<CartDetail> CartsDetails { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<Vendor> Vendors { get; set; }
		public DbSet<VendorIdentityImagesUrl> vendorIdentityImagesUrls { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		public DbSet<UserPdfBook> userPdfBooks { get; set; }


		#endregion





	}
}
