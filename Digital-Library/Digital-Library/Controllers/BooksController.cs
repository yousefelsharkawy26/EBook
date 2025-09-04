using Digital_Library.Core.Filters;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Digital_Library.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ICategoryService categoryService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _logger = logger;
        }

        // GET: Books
        public async Task<IActionResult> Index([FromQuery] BookFilter? filter)
        {
            _logger.LogInformation("Accessing Books Index with filter: {@Filter}", filter);
            var books = await _bookService.GetAllBooks(filter);

            // You might want to pass the filter back to the view to pre-fill search fields
            ViewBag.CurrentFilter = filter;
            return View(books);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details action called with null ID.");
                return NotFound();
            }

            var result = await _bookService.GetBookById(id);
            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning("Book with ID '{Id}' not found for details.", id);
                return NotFound();
            }

            return View(result.Data as Book);
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesDropdown();
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookRequest request)
        {
            // In a real application, get the VendorId from the authenticated user
            // For now, using a placeholder.
            string vendorId = "placeholderVendorId";
            // Example: var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (ModelState.IsValid)
            {
                var result = await _bookService.AddBook(request, vendorId);
                if (result.Success)
                {
                    _logger.LogInformation("Book '{Title}' created successfully by vendor {VendorId}.", request.Title, vendorId);
                    TempData["SuccessMessage"] = result.Message; // For success messages to the user
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message); // Add service-level errors to model state
                }
            }

            _logger.LogWarning("Attempted to create book '{Title}' but model state was invalid or service failed.", request.Title);
            await PopulateCategoriesDropdown(request.CategoryID); // Repopulate dropdown if validation fails
            return View(request); // Return to the form with validation errors
        }

        // GET: Books/Edit/5
        // Displays the form to edit an existing book
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit action called with null ID.");
                return NotFound();
            }

            var result = await _bookService.GetBookById(id);
            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning("Book with ID '{Id}' not found for editing.", id);
                return NotFound();
            }

            var book = result.Data as Book;


            var request = new BookRequest
            {
                Title = book.Title,
                Author = book.Author,
                PricePhysical = book.PricePhysical,
                PricePDFPerDay = book.PricePDFPerDay,
                Description = book.Description,
                Stock = book.Stock,
                HasPDF = book.HasPDF,
                IsBorrowable = book.IsBorrowable,
                CategoryID = book.CategoryID,
            };

            await PopulateCategoriesDropdown(book.CategoryID);
            return View(request);
        }

        // POST: Books/Edit/5
        // Handles the submission of the edited book form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, BookRequest request)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit POST action called with null ID.");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _bookService.UpdateBook(id, request);
                if (result.Success)
                {
                    _logger.LogInformation("Book '{Id}' updated successfully.", id);
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            _logger.LogWarning("Attempted to update book '{Id}' but model state was invalid or service failed.", id);
            await PopulateCategoriesDropdown(request.CategoryID);
            return View(request);
        }

        // GET: Books/Delete/5
        // Displays a confirmation page before deleting a book
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete GET action called with null ID.");
                return NotFound();
            }

            var result = await _bookService.GetBookById(id);
            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning("Book with ID '{Id}' not found for deletion confirmation.", id);
                return NotFound();
            }

            return View(result.Data as Book);
        }

        // POST: Books/Delete/5
        // Handles the actual deletion of the book
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete POST action called with null ID.");
                return NotFound();
            }

            var result = await _bookService.DeleteBook(id);
            if (result.Success)
            {
                _logger.LogInformation("Book '{Id}' deleted successfully.", id);
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _logger.LogError("Error deleting book '{Id}': {ErrorMessage}", id, result.Message);
                TempData["ErrorMessage"] = result.Message;

                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }


        private async Task PopulateCategoriesDropdown(string? selectedCategory = null)
        {

            var categories = await _categoryService.GetAllCategories();
            ViewBag.CategoryID = new SelectList(categories, "Id", "CategoryName", selectedCategory);
        }
    }
}