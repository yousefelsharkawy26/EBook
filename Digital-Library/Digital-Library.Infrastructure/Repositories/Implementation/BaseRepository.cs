using Digital_Library.Infrastructure.Context;
using Digital_Library.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Infrastructure.Repositories.Implementation
{
	public class BaseRepository<T> : IBaseRepository<T> where T : class
	{
		protected readonly EBookContext _context;

		public BaseRepository(EBookContext context)
		{
			_context = context;
		}
		/// <summary>
		/// استرجاع جميع الكيانات كـ IQueryable لدعم LINQ و include متقدم
		/// </summary>
		public IQueryable<T> GetAllQuery(
						Expression<Func<T, object>>[] includes = null,
						Func<IQueryable<T>, IIncludableQueryable<T, object>>[] thenIncludes = null,
						Expression<Func<T, object>> orderBy = null,
						bool orderByDescending = false)
		{
			IQueryable<T> query = _context.Set<T>();

			if (includes != null)
				foreach (var include in includes)
					query = query.Include(include);

			if (thenIncludes != null)
				foreach (var thenInclude in thenIncludes)
					query = thenInclude(query);

			if (orderBy != null)
				query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

			return query;
		}

		/// <summary>
		/// استرجاع كيان واحد حسب الـ ID
		/// </summary>
		public async Task<T> GetByIdAsync(string id)=>
	 await _context.Set<T>().FindAsync(id);
		

		/// <summary>
		/// استرجاع كيان واحد حسب شرط محدد مع إمكانية include للعلاقات
		/// </summary>
		public async Task<T> GetSingleAsync(
						Expression<Func<T, bool>> predicate,
						params Expression<Func<T, object>>[] includes)
		{
			IQueryable<T> query = _context.Set<T>();
			if (includes != null)
				foreach (var include in includes)
					query = query.Include(include);

			return await query.SingleOrDefaultAsync(predicate);
		}

		/// <summary>
		/// استرجاع كيان واحد مع include مخصص عبر Lambda
		/// </summary>
		public async Task<T> GetSingleWithIncludeAsync(
						Expression<Func<T, bool>> predicate,
						Func<IQueryable<T>, IQueryable<T>> includeFunc)
		{
			IQueryable<T> query = _context.Set<T>();
			if (includeFunc != null)
				query = includeFunc(query);

			return await query.SingleOrDefaultAsync(predicate);
		}

		/// <summary>
		/// استرجاع قائمة كيانات حسب شرط محدد مع include للعلاقات
		/// </summary>
		public async Task<IEnumerable<T>> GetManyAsync(
						Expression<Func<T, bool>> predicate,
						params Expression<Func<T, object>>[] includes)
		{
			IQueryable<T> query = _context.Set<T>();
			if (includes != null)
				foreach (var include in includes)
					query = query.Include(include);

			return await query.Where(predicate).ToListAsync();
		}

		/// <summary>
		/// استرجاع قائمة كيانات كـ IQueryable لدعم الاستعلامات المعقدة قبل التنفيذ
		/// </summary>
		public IQueryable<T> GetManyQuery(
						Expression<Func<T, bool>> predicate,
						Expression<Func<T, object>>[] includes = null,
						Func<IQueryable<T>, IIncludableQueryable<T, object>>[] thenIncludes = null)
		{
			IQueryable<T> query = _context.Set<T>().Where(predicate);

			if (includes != null)
				foreach (var include in includes)
					query = query.Include(include);

			if (thenIncludes != null)
				foreach (var thenInclude in thenIncludes)
					query = thenInclude(query);

			return query;
		}

		// =========================================
		// Adding
		// =========================================
		public async Task<T> AddAsync(T entity)
		{
			await _context.Set<T>().AddAsync(entity);
			return entity;
		}

		public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
		{
			await _context.Set<T>().AddRangeAsync(entities);
			return entities;
		}

		// =========================================
		// Updating
		// =========================================
		public T Update(T entity)
		{
			_context.Update(entity);
			return entity;
		}

		// =========================================
		// Deleting
		// =========================================
		public void Delete(T entity) => _context.Set<T>().Remove(entity);
		public void DeleteRange(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);


		// =========================================
		// Counting
		// =========================================
		public async Task<int> CountAsync() => await _context.Set<T>().CountAsync();
		public async Task<int> CountAsync(Expression<Func<T, bool>> predicate) =>
						await _context.Set<T>().CountAsync(predicate);

	}

}
