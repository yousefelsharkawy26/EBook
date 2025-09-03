using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Infrastructure.Repositories.Interface
{
	public interface IBaseRepository<T> where T : class
	{
		/// <summary>
		/// استرجاع جميع الكيانات كـ IQueryable لدعم LINQ.
		/// </summary>
		IQueryable<T> GetAllQuery(
						Expression<Func<T, object>>[] includes = null,
						Func<IQueryable<T>, IIncludableQueryable<T, object>>[] thenIncludes = null,
						Expression<Func<T, object>> orderBy = null,
						bool orderByDescending = false);

		/// <summary>
		/// استرجاع كيان واحد حسب المعرف (ID).
		/// </summary>
		Task<T> GetByIdAsync(string id);

		/// <summary>
		/// استرجاع كيان واحد حسب شرط معين مع إمكانية include للعلاقات.
		/// </summary>
		Task<T> GetSingleAsync(
						Expression<Func<T, bool>> predicate,
						params Expression<Func<T, object>>[] includes);

		/// <summary>
		/// استرجاع قائمة كيانات حسب شرط معين مع إمكانية include للعلاقات.
		/// </summary>
		Task<IEnumerable<T>> GetManyAsync(
						Expression<Func<T, bool>> predicate,
						params Expression<Func<T, object>>[] includes);

		/// <summary>
		/// استرجاع قائمة كيانات حسب شرط معين كـ IQueryable لدعم LINQ و include متقدم.
		/// </summary>
		IQueryable<T> GetManyQuery(
						Expression<Func<T, bool>> predicate,
						Expression<Func<T, object>>[] includes = null,
						Func<IQueryable<T>, IIncludableQueryable<T, object>>[] thenIncludes = null);

		/// <summary>
		/// استرجاع كيان واحد حسب شرط معين مع include مخصص عبر Lambda function.
		/// </summary>
		Task<T> GetSingleWithIncludeAsync(
						Expression<Func<T, bool>> predicate,
						Func<IQueryable<T>, IQueryable<T>> includeFunc);

		// =========================================
		// Adding / الإضافة
		// =========================================
		Task<T> AddAsync(T entity);
		Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

		// =========================================
		// Updating / التعديل
		// =========================================
		T Update(T entity);

		// =========================================
		// Deleting / الحذف
		// =========================================
		void Delete(T entity);
		void DeleteRange(IEnumerable<T> entities);

		// =========================================
		// Counting / العد
		// =========================================
		Task<int> CountAsync();
		Task<int> CountAsync(Expression<Func<T, bool>> predicate);
	}
}
