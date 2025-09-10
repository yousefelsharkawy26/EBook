using Digital_Library.Core.ViewModels;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;


namespace Digital_Library.AdminPanel.Controllers;
public class BooksController : Controller
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // GET: /Books
    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllBooksAsync();
        return View(books);
    }

    // GET: /Books/Create
    public async Task<IActionResult> Create()
    {
        var viewModel = await _bookService.GetBookCreationDataAsync();
        return View(viewModel);
    }

    // POST: /Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = (await _bookService.GetBookCreationDataAsync()).Categories;
            model.Vendors = (await _bookService.GetBookCreationDataAsync()).Vendors;
            return View(model);
        }
        await _bookService.CreateBookAsync(model);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Books/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        var viewModel = await _bookService.GetBookForEditAsync(id);
        if (viewModel == null) return NotFound();
        return View(viewModel);
    }

    // POST: /Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookFormViewModel model, IFormFile? CoverImage)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = (await _bookService.GetBookCreationDataAsync()).Categories;
            model.Vendors = (await _bookService.GetBookCreationDataAsync()).Vendors;
            return View(model);
        }
        model.CoverImage = CoverImage;
        await _bookService.UpdateBookAsync(model);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Books/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        var viewModel = await _bookService.GetBookForEditAsync(id);
        if (viewModel == null) return NotFound();
        return View(viewModel);
    }

    // POST: /Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _bookService.DeleteBook(id);
        return RedirectToAction(nameof(Index));
    }
}
