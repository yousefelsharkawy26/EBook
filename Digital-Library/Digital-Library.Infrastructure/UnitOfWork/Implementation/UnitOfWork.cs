using Digital_Library.Core.Models;
using Digital_Library.Infrastructure.Context;
using Digital_Library.Infrastructure.Repositories.Implementation;
using Digital_Library.Infrastructure.Repositories.Interface;
using Digital_Library.Infrastructure.UnitOfWork.Interface;

namespace Digital_Library.Infrastructure.UnitOfWork.Implementation
{
	public class UnitOfWork : IUnitOfWork
	{
		public EBookContext _context { get; }

        public IBaseRepository<User> Users { get; }
        public IBaseRepository<Book> Books { get; }
        public IBaseRepository<Borrowing> Borrowings { get; }
        public IBaseRepository<Cart> Carts { get; }
        public IBaseRepository<CartDetail> CartDetails { get; }
        public IBaseRepository<Category> Categories { get; }
        public IBaseRepository<Order> Orders { get; }
        public IBaseRepository<OrderDetails> OrderDetails { get; }
        public IBaseRepository<Transaction> Transactions { get; }
        public IBaseRepository<Vendor> Vendors { get; }

        public UnitOfWork(EBookContext context)
		{
			_context = context;
			Users = new BaseRepository<User>(_context);
			Books = new BaseRepository<Book>(_context);
			Borrowings = new BaseRepository<Borrowing>(_context);
			Carts = new BaseRepository<Cart>(_context);
			Categories = new BaseRepository<Category>(_context);
			Orders = new BaseRepository<Order>(_context);
			OrderDetails = new BaseRepository<OrderDetails>(_context);
			Transactions = new BaseRepository<Transaction>(_context);
			Vendors = new BaseRepository<Vendor>(_context);
		}
		public void Dispose()
		{
			_context.Dispose();
		}
		public async Task BeginTransactionAsync()
		{
			await _context.Database.BeginTransactionAsync();
		}
		public async Task RolleBack()
		{
			await _context.Database.RollbackTransactionAsync();
		}
		public async Task Commit()
		{
			await _context.Database.CommitTransactionAsync();
		}
		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
