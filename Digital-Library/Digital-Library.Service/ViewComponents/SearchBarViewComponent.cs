using Digital_Library.Core.Models;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.ViewComponents
{
	public class SearchBarViewComponent : ViewComponent
	{
		private readonly IBookService _bookService;
		private readonly IMemoryCache _cache;

		public SearchBarViewComponent(IBookService bookService, IMemoryCache cache)
		{
			_bookService = bookService;
			_cache = cache;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var categories = await _cache.GetOrCreateAsync("RandomCategories", async entry =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(120); 

				var books = await _bookService.GetAllBooks().AsNoTracking().ToListAsync();

				return books
								.Where(b => b.Category != null)
								.Select(b => b.Category!)
								.GroupBy(c => c.Id)
								.Select(g => g.First())
								.OrderBy(_ => Guid.NewGuid())
								.Take(8)
								.ToList();
			});

			return View("Default", categories);
		}
	}

}

