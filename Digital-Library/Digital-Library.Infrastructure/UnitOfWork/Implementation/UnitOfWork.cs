using Digital_Library.Infrastructure.Context;
using Digital_Library.Infrastructure.Repositories.Implementation;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Infrastructure.UnitOfWork.Implementation
{
	public class UnitOfWork : IUnitOfWork
	{
		public EBookContext _context { get; }
		public UnitOfWork(EBookContext context)
		{
			_context = context;
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
