using Digital_Library.Core.Models;
using Digital_Library.Infrastructure.Repositories.Interface;

namespace Digital_Library.Infrastructure.UnitOfWork.Interface;
public interface IUnitOfWork : IDisposable
{
	IBaseRepository<User> Users {  get; }
	IBaseRepository<Book> Books {  get; }
	IBaseRepository<Borrowing> Borrowings {  get; }
	IBaseRepository<Cart> Carts {  get; }
	IBaseRepository<CartDetail> CartDetails {  get; }
	IBaseRepository<Category> Categories {  get; }
	IBaseRepository<Order> Orders {  get; }
	IBaseRepository<OrderDetail> OrderDetails {  get; }
	IBaseRepository<OrderHeader> OrderHeaders {  get; }
	IBaseRepository<Transaction> Transactions {  get; }
	IBaseRepository<Vendor> Vendors {  get; }

	Task SaveChangesAsync();
	Task Commit();
	Task RolleBack();
	Task BeginTransactionAsync();
}
