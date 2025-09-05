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

			return RedirectToAction("Index", "Books");
		}

		[HttpGet("Edit/{id}")]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> EditBook(string id)
		{
			var bookResponse = await bookService.GetBookById(id);
			if (!bookResponse.Success)
			{
				return NotFound();
			}
			var categories = await categoryService.GetAllCategories();
			ViewBag.Categories = categories.Select(c => new SelectListItem
			{
				Value = c.Id,
				Text = c.CategoryName
			}).ToList();
			return View(bookResponse);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Roles.Vendor)]
		public async Task<IActionResult> EditBook(string id, BookRequest request)
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
			var response = await bookService.UpdateBook(id, request);
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
			return RedirectToAction("Index", "Books");
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
			return View(bookResponse);
		}
	}
}