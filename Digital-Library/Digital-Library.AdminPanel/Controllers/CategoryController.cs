using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Digital_Library.AdminPanel.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ICategoryService categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			this.categoryService = categoryService;
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			return View(await categoryService.GetAllCategories());
		}
		[HttpGet()]
		public async Task<IActionResult> Update(string categoryId)
		{
			if (string.IsNullOrEmpty(categoryId))
				return NotFound();

			var res = await categoryService.GetCategoryById(categoryId);
			var category = res.Data as Category;
			if (category == null)
				return NotFound();
			var request = new CategoryRequest
			{
				CategoryName = category.CategoryName,
				Description = category.Description
			};
			return View(request);
		}

		[HttpPost()]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(string id,CategoryRequest request)
		{
			if (!ModelState.IsValid)
			{
				return View("Update", request);
			}

			var result = await categoryService.UpdateCategory(id, request);

			if (result.Success)
			{
				TempData["SuccessMessage"] = result.Message;
				return RedirectToAction("Index");
			}

			ModelState.AddModelError(string.Empty, result.Message);
			return View("Update", request);
		}



	}
}
