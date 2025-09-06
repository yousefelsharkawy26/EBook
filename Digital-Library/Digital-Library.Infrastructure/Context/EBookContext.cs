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

			modelBuilder.Entity<Borrowing>()
							.HasOne(b => b.Book)
							.WithMany(book => book.Borrowings)
							.HasForeignKey(b => b.BookId)
							.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<CartDetail>()
						.HasOne(cd => cd.Cart)
						.WithMany(c => c.CartDetails)
						.HasForeignKey(cd => cd.CartId)
						.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<OrderDetail>()
				.HasOne(od => od.OrderHeader)
				.WithMany(o => o.OrderDetails)
				.HasForeignKey(od => od.OrderHeaderId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<OrderHeader>()
				.HasOne(od => od.Vendor)
				.WithMany(b => b.OrderHeaders)
				.HasForeignKey(od => od.VendorId)
				.OnDelete(DeleteBehavior.Restrict);
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
		#endregion





	}
}
