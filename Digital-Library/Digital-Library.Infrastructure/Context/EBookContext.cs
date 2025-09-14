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

			modelBuilder.Entity<UserBookAccess>()
							.HasOne(uba => uba.User)
							.WithMany(u => u.UserBookAccesses)
							.HasForeignKey(uba => uba.UserId)
							.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<UserBookAccess>()
							.HasOne(uba => uba.Book)
							.WithMany(b => b.UserBookAccesses)
							.HasForeignKey(uba => uba.BookId)
							.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<CartDetail>()
							.HasOne(cd => cd.Cart)
							.WithMany(c => c.CartDetails)
							.HasForeignKey(cd => cd.CartId)
							.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<OrderDetail>()
							.HasOne(od => od.OrderHeader)
							.WithMany(o => o.OrderDetails)
							.HasForeignKey(od => od.OrderHeaderId)
							.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<OrderDetail>()
							.HasOne(od => od.Book)
							.WithMany(b => b.OrderDetails)
							.HasForeignKey(od => od.BookId)
							.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Book>()
							.HasOne(b => b.Category)
							.WithMany(c => c.Books)
							.HasForeignKey(b => b.CategoryID)
							.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Book>()
							.HasOne(b => b.Vendor)
							.WithMany(v => v.Books)
							.HasForeignKey(b => b.VendorId)
							.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<OrderHeader>()
				.HasOne(b => b.Vendor)
				.WithMany(v => v.OrderHeaders)
				.HasForeignKey(b => b.VendorId)
				.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<UserBookAccess>()
							.HasKey(uba => new { uba.UserId, uba.BookId });
		}

		#region Entities
		public DbSet<Book> Books { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<CartDetail> CartsDetails { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<Vendor> Vendors { get; set; }
		public DbSet<VendorIdentityImagesUrl> vendorIdentityImagesUrls { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		public DbSet<UserBookAccess> UserBookAccesses { get; set; }

		#endregion





	}
}
