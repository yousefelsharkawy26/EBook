using Digital_Library.Core.Constant;
using Digital_Library.Core.Filters;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Digital_Library.Controllers
{
	public class BookController : Controller
	{
		private readonly IBookService bookService;
		private readonly ICategoryService categoryService;
		private readonly IVendorService vendorService;

		public BookController(IBookService bookService, ICategoryService categoryService, IVendorService vendorService)
		{
			this.bookService = bookService;
			this.categoryService = categoryService;
			this.vendorService = vendorService;
		}
		[HttpGet]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> Index()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var vendorId=	await vendorService.ReturnVendorIdFromUserId(userId);
			if (!vendorId.Success)
			{
				return BadRequest();
			}
			var books = await bookService.GetAllBooks(new BookFilter { VendorId = (string)vendorId.Data });

			return View(books);
		}

		[HttpGet]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> AddBook()
		{
			var categories = await categoryService.GetAllCategories();
			ViewBag.Categories = categories.Select(c => new SelectListItem
			{
				Value = c.Id,
				Text = c.CategoryName
			}).ToList();
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> AddBook(BookRequest request)
		{
			if (!ModelState.IsValid)
			{
				var categories = await categoryService.GetAllCategories();
				ViewBag.Categories = categories.Select(c => new SelectListItem
				{
					Value = c.Id,
					Text = c.CategoryName
				}).ToList();

				return View(request);
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var vendorId = await vendorService.ReturnVendorIdFromUserId(userId);
			if (!vendorId.Success)
			{
				return BadRequest();
			}
			var response = await bookService.AddBook(request, vendorId.Data.ToString());

			if (!response.Success)
			{
				ModelState.AddModelError("", response.Message);
				var categories = await categoryService.GetAllCategories();
				ViewBag.Categories = categories.Select(c => new SelectListItem
				{
					Value = c.Id,
					Text = c.CategoryName
				}).ToList();

				return View(request);
			}

			return RedirectToAction("Index", "Book");
		}

		[HttpGet("Edit/{id}")]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> EditBook(string id)
		{
			var response = await bookService.GetBookById(id);
			if (!response.Success || response.Data == null)
				return NotFound(response.Message);

			var book = response.Data as Book; 

			if (book == null)
				return NotFound("Book data not found");


			var request = new UpdateBookRequest
			{
				BookID=book.Id,
				Title = book.Title,
				Author = book.Author,
				PricePhysical = book.PricePhysical,
				PricePDF = book.PricePdf,
				PricePDFPerDay = book.PricePDFPerDay,
				Description = book.Description,
				Stock = book.Stock,
				HasPDF = book.HasPDF,
				IsBorrowable = book.IsBorrowable,
				CategoryID = book.CategoryID
			};

			var categories = await categoryService.GetAllCategories();
			ViewBag.Categories = categories.Select(c => new SelectListItem
			{
				Value = c.Id,
				Text = c.CategoryName
			}).ToList();

			return View(request); 
		}

		[HttpPost("Edit/{id}")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> EditBook(UpdateBookRequest request)
		{
			if (!ModelState.IsValid)
			{
				var categories = await categoryService.GetAllCategories();
				ViewBag.Categories = categories.Select(c => new SelectListItem
				{
					Value = c.Id,
					Text = c.CategoryName
				}).ToList();
				return View(request);
			}
			var response = await bookService.UpdateBook(request.BookID, request );
			if (!response.Success)
			{
				ModelState.AddModelError("", response.Message);
				var categories = await categoryService.GetAllCategories();
				ViewBag.Categories = categories.Select(c => new SelectListItem
				{
					Value = c.Id,
					Text = c.CategoryName
				}).ToList();
				return View(request);
			}
			return RedirectToAction("Index", "Book");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> DeleteBook(string id)
		{
			var response = await bookService.DeleteBook(id);

			if (!response.Success)
			{
				TempData["ErrorMessage"] = response.Message;
				return RedirectToAction("Index", "Books");
			}

			TempData["SuccessMessage"] = "Book deleted successfully.";
			return RedirectToAction("Index", "Books");
		}

		[HttpGet("Details/{id}")]
		public async Task<IActionResult> Details(string id)
		{
			var bookResponse = await bookService.GetBookById(id);
			if (!bookResponse.Success)
			{
				return NotFound();
			}
			return View(bookResponse.Data as Book);
		}
	}
}