using Digital_Library.Core.Constant;
using Digital_Library.Core.Filters;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Digital_Library.Controllers
{
	[Route("Book")]
	public class BookController : Controller
	{
		private readonly IBookService bookService;
		private readonly ICategoryService categoryService;
		private readonly IVendorService vendorService;
		private readonly IFileService fileService;

		public BookController(IBookService bookService, ICategoryService categoryService, IVendorService vendorService , IFileService fileService)
		{
			this.bookService = bookService;
			this.categoryService = categoryService;
			this.vendorService = vendorService;
			this.fileService = fileService;
		}
		[HttpGet("Index")]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> Index()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var vendorId = await vendorService.ReturnVendorIdFromUserId(userId);
			if (!vendorId.Success)
			{
				return BadRequest();
			}
			var books = await bookService.GetAllBooks(new BookFilter { VendorId = (string)vendorId.Data });

			return View(books);
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
				TempData["ErrorMessage"] = response.Message;
				return RedirectToAction("Index", "Books");
			}

			TempData["SuccessMessage"] = "Book deleted successfully.";
			return RedirectToAction("Index", "Book");
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

        [HttpGet("allbooks")]
        public async Task<IActionResult> ViewAllBooks(string category, string author, string priceRange, string sort)
        {
            var books = await bookService.GetAllBooks();

            // Filter by category
            if (!string.IsNullOrEmpty(category))
                books = books.Where(b => b.CategoryID == category).ToList();

            // Filter by author
            if (!string.IsNullOrEmpty(author))
                books = books.Where(b => b.Author == author).ToList();

            // Filter by price range
            if (!string.IsNullOrEmpty(priceRange))
            {
                var parts = priceRange.Split('-');
                if (parts.Length == 2 &&
                    decimal.TryParse(parts[0], out decimal minPrice) &&
                    decimal.TryParse(parts[1], out decimal maxPrice))
                {
                    books = books.Where(b => b.PricePhysical >= minPrice && b.PricePhysical <= maxPrice).ToList();
                }
            }

            // Sorting (optional)
            books = sort switch
            {
                "NameAsc" => books.OrderBy(b => b.Title).ToList(),
                "NameDesc" => books.OrderByDescending(b => b.Title).ToList(),
                "PriceLowHigh" => books.OrderBy(b => b.PricePhysical).ToList(),
                "PriceHighLow" => books.OrderByDescending(b => b.PricePhysical).ToList(),
                _ => books
            };

            // Populate viewbags
            ViewBag.Categories = await categoryService.GetAllCategories();
            ViewBag.Authors = books.Select(b => b.Author).Distinct().ToList();


            return View(books);
        }


    }
}