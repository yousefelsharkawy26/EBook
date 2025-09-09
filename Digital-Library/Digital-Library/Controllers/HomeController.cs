using Digital_Library.Core.ViewModels;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Models;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Digital_Library.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ICategoryService _categoryService;
		private readonly IBookService bookService;

		public HomeController(ILogger<HomeController> logger, ICategoryService categoryService ,IBookService bookService)
		{
			_logger = logger;
			_categoryService = categoryService;
			this.bookService = bookService;
		}

		public async Task<IActionResult> Index()
		{
			return View(await GetRandomCategories());
		}
		[HttpGet]
		private async Task<HomeViewModel> GetRandomCategories()
		{
			var books = await bookService
				.GetAllBooks()
				.AsNoTracking()
				.ToListAsync();

			var categories = books
							.Where(b => b.Category != null)
							.Select(b => b.Category!)
							.GroupBy(c => c.Id)
							.Select(g => g.First())
							.OrderBy(_ => Guid.NewGuid())
							.Take(8)
							.ToList();
			return new HomeViewModel
			{
					RandomCategories = categories,
					Books = books
				
			};
		}
		public IActionResult About()
		{
			return View();
		}

		public IActionResult Contact()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
