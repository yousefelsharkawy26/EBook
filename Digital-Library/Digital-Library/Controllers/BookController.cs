using Digital_Library.Core.Constant;
using Digital_Library.Core.Filters;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
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
        //[HttpGet("Index")]
        //[Authorize(Roles = Roles.Vendor)]
        //public async Task<IActionResult> Index()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var vendorId = await vendorService.ReturnVendorIdFromUserId(userId);
        //    if (!vendorId.Success)
        //    {
        //        return BadRequest();
        //    }
        //    var books = await bookService.GetAllBooks(new BookFilter { VendorId = (string)vendorId.Data });

        //    return View(books);
        //}
        [HttpGet("Index")]
        [Authorize(Roles = Roles.Vendor)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var (books, totalCount) = await bookService.GetPagedBooksAsync(page, pageSize);

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
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction("Index", "Books");
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

            // Fetch related books (same category, excluding current)
            var response = await bookService.GetAllBooks();
            var relatedBooks = response
                .Where(b => b.CategoryID == book.CategoryID && b.Id != book.Id)
                .Take(3)
                .ToList();

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

            // Fetch related books (same category, excluding current)
            var response = await bookService.GetAllBooks();
            var relatedBooks = response
                .Where(b => b.CategoryID == book.CategoryID && b.Id != book.Id)
                .Take(3)
                .ToList();

            var viewModel = new BookDetailsViewModel
            {
                Book = book,
                RelatedBooks = relatedBooks
            };

            return View(viewModel);
        }





        [HttpGet("ShowPdf/{id}")]
        [Authorize]
        public async Task<IActionResult> ShowPdf(string id)
        {
            if (TempData["IsAllowed"] == null)
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var res = await bookService.GetBookById(id);
            var book = res.Data as Book;
            if (book == null || !book.HasPDF || string.IsNullOrEmpty(book.PDFFilePath))
                return NotFound("PDF not available");

            ViewBag.PdfPath = book.PDFFilePath;
            
            TempData.Remove("IsAllowed");
            return View();
        }

        [HttpGet("allbooks")]
        public async Task<IActionResult> ViewAllBooks(
    string category,
    string author,
    string priceRange,
    string sort,
    string query,   // 👈 added search term
    int page = 1)
        {
            int pageSize = 8;

            var books = await bookService.GetAllBooks();

            // 🔎 Search filter
            if (!string.IsNullOrEmpty(query))
            {
                books = books.Where(b =>
                    (!string.IsNullOrEmpty(b.Title) && b.Title.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(b.Author) && b.Author.Contains(query, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Category filter
            if (!string.IsNullOrEmpty(category))
                books = books.Where(b => b.CategoryID == category).ToList();

            // Author filter
            if (!string.IsNullOrEmpty(author))
                books = books.Where(b => b.Author == author).ToList();

            // Price filter
            if (!string.IsNullOrEmpty(priceRange))
            {
                var parts = priceRange.Split('-');
                if (parts.Length == 2 &&
                    decimal.TryParse(parts[0], out decimal minPrice) &&
                    decimal.TryParse(parts[1], out decimal maxPrice))
                {
                    books = books
                        .Where(b => b.PricePhysical >= minPrice && b.PricePhysical <= maxPrice)
                        .ToList();
                }
            }

            // Sorting
            books = sort switch
            {
                "NameAsc" => books.OrderBy(b => b.Title).ToList(),
                "NameDesc" => books.OrderByDescending(b => b.Title).ToList(),
                "PriceLowHigh" => books.OrderBy(b => b.PricePhysical).ToList(),
                "PriceHighLow" => books.OrderByDescending(b => b.PricePhysical).ToList(),
                _ => books
            };

            // Pagination
            var totalItems = books.Count();
            var items = books.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.Categories = await categoryService.GetAllCategories();
            ViewBag.Authors = books.Select(b => b.Author).Distinct().ToList();

            return View(items);
        }


        [HttpGet("ShowMyBook")]
        [Authorize]
        public async Task<IActionResult> ShowMyBook()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var books = await bookService.GetMyBook(userId);

            if (books.Any())
                TempData["IsAllowed"] = true;
                


            return View(books);
        }


        [HttpGet("Search")]
        public async Task<IActionResult> Search(string query, int page = 1, int pageSize = 8)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                // If no search keyword, just show all books
                return RedirectToAction("ViewAllBooks");
            }

            // Call service with filter
            var filter = new BookFilter
            {
                Keyword = query
            };

            var allBooks = await bookService.GetAllBooks(filter);

            // Pagination
            var totalItems = allBooks.Count();
            var items = allBooks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Query = query;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            return View("SearchResults", items); // 👈 Create a SearchResults.cshtml
        }


    }
}