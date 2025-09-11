using Digital_Library.Core.Constant;
using Digital_Library.Core.Enums;
using Digital_Library.Core.Filters;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Digital_Library.Controllers
{
	[Route("Book")]
	public class BookController : Controller
	{
		private readonly IBookService bookService;
		private readonly ICategoryService categoryService;
		private readonly IVendorService vendorService;
		private readonly ICartService cartService;
		private readonly IFileService fileService;

		public BookController(IBookService bookService, ICategoryService categoryService, IVendorService vendorService, IFileService fileService, ICartService cartService)
		{
			this.bookService = bookService;
			this.categoryService = categoryService;
			this.vendorService = vendorService;
			this.fileService = fileService;
			this.cartService = cartService;
		}

		[HttpGet("Index")]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var vendorId = await vendorService.ReturnVendorIdFromUserId(userId);
			if (!vendorId.Success)
			{
				return BadRequest();
			}
			var (books, totalCount) = await bookService.GetPagedBooksAsync(vendorId.Data.ToString(), page, pageSize);

			int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;
			ViewBag.PageSize = pageSize;

			return View(books);
		}


		[HttpGet("ConfirmDelete/{id}")]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> ConfirmDelete(string id)
		{
			var response = await bookService.GetBookById(id);
			if (!response.Success || response.Data is not Book book)
			{
				return NotFound();
			}

			return View(book);
		}


		[HttpGet("AddBook")]
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

		[HttpPost("AddBook")]
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
				BookID = book.Id,
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
			var response = await bookService.UpdateBook(request.BookID, request);
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
				TempData["ErrorMessage"] = "Can not Delete Book";
				return RedirectToAction("Index", "Book");
			}

			TempData["SuccessMessage"] = "Book deleted successfully.";
			return RedirectToAction("Index", "Book");
		}

		[HttpGet("MyDetails/{id}")]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> MyDetails(string id)
		{
			var bookResponse = await bookService.GetBookById(id);
			if (!bookResponse.Success || bookResponse.Data is not Book book)
			{
				return NotFound();
			}

			var relatedBooks = await bookService.GetRelatedBooksAsync( book.Id);

			var viewModel = new BookDetailsViewModel
			{
				Book = book,
				RelatedBooks = relatedBooks
			};

			return View(viewModel);
		}

		[HttpGet("Details/{id}")]
		public async Task<IActionResult> Details(string id)
		{
			var bookResponse = await bookService.GetBookById(id);
			if (!bookResponse.Success || bookResponse.Data is not Book book)
			{
				return NotFound();
			}

			var relatedBooks = await bookService.GetRelatedBooksAsync(book.Id);

			var viewModel = new BookDetailsViewModel
			{
				Book = book,
				RelatedBooks = relatedBooks
			};

			return View(viewModel);
		}


		[HttpGet("ShowPdf/{id}")]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> ShowPdf(string id)
		{
			if (string.IsNullOrEmpty(id))
				return NotFound();

			var res = await bookService.GetBookById(id);
			var book = res.Data as Book;
			if (book == null || !book.HasPDF || string.IsNullOrEmpty(book.PDFFilePath))
				return NotFound("PDF not available");

			ViewBag.PdfPath = book.PDFFilePath;
			return View();
		}

		[HttpGet("AllBooks")]
		public async Task<IActionResult> ViewAllBooks(
						string category,
						string author,
						string priceRange,
						string sort,
						string query,
						int page = 1)
		{
			int pageSize = 8;

			IQueryable<Book> booksQuery = bookService.GetAllBooks();

			if (!string.IsNullOrEmpty(query))
			{
				booksQuery = booksQuery.Where(b =>
								(!string.IsNullOrEmpty(b.Title) && b.Title.Contains(query)) ||
								(!string.IsNullOrEmpty(b.Author) && b.Author.Contains(query)));
			}

			if (!string.IsNullOrEmpty(category))
				booksQuery = booksQuery.Where(b => b.CategoryID == category);

			if (!string.IsNullOrEmpty(author))
				booksQuery = booksQuery.Where(b => b.Author == author);

			if (!string.IsNullOrEmpty(priceRange))
			{
				var parts = priceRange.Split('-');
				if (parts.Length == 2 &&
								decimal.TryParse(parts[0], out decimal minPrice) &&
								decimal.TryParse(parts[1], out decimal maxPrice))
				{
					booksQuery = booksQuery.Where(b => b.PricePhysical >= minPrice && b.PricePhysical <= maxPrice);
				}
			}
			booksQuery = sort switch
			{
				"NameAsc" => booksQuery.OrderBy(b => b.Title),
				"NameDesc" => booksQuery.OrderByDescending(b => b.Title),
				"PriceLowHigh" => booksQuery.OrderBy(b => b.PricePhysical),
				"PriceHighLow" => booksQuery.OrderByDescending(b => b.PricePhysical),
				_ => booksQuery.OrderBy(b => b.Title)
			};
			var totalItems = await booksQuery.CountAsync();
			var items = await booksQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
			ViewBag.PageSize = pageSize;
			ViewBag.TotalItems = totalItems;

			ViewBag.Categories = await categoryService.GetAllCategories();
			ViewBag.Authors = await booksQuery.Select(b => b.Author).Distinct().ToListAsync();

			return View(items);
		}

		[HttpGet("MyBooks")]
		[Authorize]
		public async Task<IActionResult> MyBooks(int page = 1, int pageSize = 10)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized();
			var result = await bookService.GetUserBooksAsync(userId, page, pageSize);

			return View(result);
		}
		[Authorize]
		[HttpGet("ReadBook/{id}")]
		public async Task<IActionResult> ReadBook(string id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized();

			// تحقق من صلاحية المستخدم
			var accessType = await bookService.GetUserBookAccessAsync(userId, id);
			if (accessType == UserBookAccessType.None)
				return Forbid();

			var res = await bookService.GetBookById(id);
			var book = res.Data as Book;
			if (book == null)
				return NotFound();

			return View(new ReadBookViewModel
			{
				BookId = book.Id,
				Title = book.Title,
				FilePath = book.PDFFilePath,
				IsBorrowed = accessType == UserBookAccessType.Borrowed,
				CanDownload = accessType == UserBookAccessType.Purchased,
				CanPrint = accessType == UserBookAccessType.Purchased
			});
		}

		[Authorize]
		[HttpGet("StreamBook/{id}")]
		public async Task<IActionResult> StreamBook(string id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized();

			var accessType = await bookService.GetUserBookAccessAsync(userId, id);
			if (accessType == UserBookAccessType.None)
				return Forbid();

			var res = await bookService.GetBookById(id);
			var book = res.Data as Book;
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.PDFFilePath);
			if (!System.IO.File.Exists(filePath))
				return NotFound();

			var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			return File(fs, "application/pdf", enableRangeProcessing: true);
		}

		[Authorize]
		[HttpGet("DownloadBook/{id}")]
		public async Task<IActionResult> DownloadBook(string id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized();

			var accessType = await bookService.GetUserBookAccessAsync(userId, id);
			if (accessType != UserBookAccessType.Purchased)
				return Forbid(); 

			var res = await bookService.GetBookById(id);
			var book = res.Data as Book;
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.PDFFilePath);
			if (book == null || !System.IO.File.Exists(filePath))
				return NotFound();

			var fileName = Path.GetFileName(filePath);
			return PhysicalFile(filePath, "application/pdf", fileName);
		}

	}
}