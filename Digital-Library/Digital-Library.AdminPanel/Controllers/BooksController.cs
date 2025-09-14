using Digital_Library.Core.ViewModels;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;


namespace Digital_Library.AdminPanel.Controllers;
public class BooksController : Controller
{
	private readonly IBookService _bookService;
	private readonly HttpClient _httpClient;

	public BooksController(IBookService bookService, IHttpClientFactory httpClientFactory)
	{
		_bookService = bookService;
		_httpClient = httpClientFactory.CreateClient();
		_httpClient.BaseAddress = new Uri("https://zzaki213-001-site1.stempurl.com/");

	}
	public async Task<IActionResult> Index()
	{
		var books = await _bookService.GetAllBooksAsync();
		return View(books);
	}


	public async Task<IActionResult> Create()
	{
		var viewModel = await _bookService.GetBookCreationDataAsync();
		return View(viewModel);
	}
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

	[HttpGet]
	public async Task<IActionResult> Edit(string id)
	{
		var viewModel = await _bookService.GetBookForEditAsync(id);
		if (viewModel == null) return NotFound();
		return View(viewModel);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(BookFormViewModel model)
	{
		if (!ModelState.IsValid)
		{
			model.Categories = (await _bookService.GetBookCreationDataAsync()).Categories;
			model.Vendors = (await _bookService.GetBookCreationDataAsync()).Vendors;
			return View(model);
		}
		if (model.CoverImage != null && model.CoverImage.Length > 0)
		{
			using var content = new MultipartFormDataContent();
			var fileContent = new StreamContent(model.CoverImage.OpenReadStream());
			fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.CoverImage.ContentType);

			content.Add(fileContent, "file", model.CoverImage.FileName);
			content.Add(new StringContent(model.ExistingCoverImage ?? ""), "oldNameFile");

			var response = await _httpClient.PostAsync("api/File/upload", content);

			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadFromJsonAsync<UploadResult>();
				model.ExistingCoverImage = result?.FilePath;
			}
			else
			{
				ModelState.AddModelError("", "Error uploading file to server.");
				model.Categories = (await _bookService.GetBookCreationDataAsync()).Categories;
				model.Vendors = (await _bookService.GetBookCreationDataAsync()).Vendors;
				return View(model);
			}
		}

		await _bookService.UpdateBookAsync(model);
		return RedirectToAction(nameof(Index));
	}

	public async Task<IActionResult> Delete(string id)
	{
		var viewModel = await _bookService.GetBookForEditAsync(id);
		if (viewModel == null) return NotFound();
		return View(viewModel);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(string id)
	{
		await _bookService.DeleteBook(id);
		return RedirectToAction(nameof(Index));
	}
}
