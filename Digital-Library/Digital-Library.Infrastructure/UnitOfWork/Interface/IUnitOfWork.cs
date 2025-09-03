using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Infrastructure.UnitOfWork.Interface
{
	public interface IUnitOfWork : IDisposable
	{
		Task SaveChangesAsync();
		Task Commit();
		Task RolleBack();
		Task BeginTransactionAsync();
	}
}
