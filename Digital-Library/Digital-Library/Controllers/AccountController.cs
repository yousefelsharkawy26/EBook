using Digital_Library.Core.Constant;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Digital_Library.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly IFileService _fileService;
		private readonly IAuthService _authService;

		public AccountController(UserManager<User> userManager,
																										IFileService fileService,
																										IAuthService authService)
		{
			_userManager = userManager;
			_fileService = fileService;
			_authService = authService;
		}

		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			return RedirectToAction("Login", "Auth", new { returnUrl });
		}

		[HttpGet]
		public IActionResult AccessDenied()
		{
			return View();
		}
		public async Task<IActionResult> Profile()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return NotFound();

			var model = new UserProfileViewModel()
			{
				Id = user.Id,
				Email = user.Email,
				FullName = user.FullName,
				ImageUrl = user.ImageUrl,
			};


			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Profile(UserProfileViewModel model, IFormFile file)
		{
			var user = await _userManager.FindByIdAsync(model.Id);
			user.FullName = model.FullName;
			var isEmailUpdate = false;
			if (user.Email != model.Email)
			{
				user.Email = model.Email;
				isEmailUpdate = true;
			}

			var imageUrl = await _fileService.UpdateFile(file, user.ImageUrl ?? "");

			user.ImageUrl = imageUrl;
			model.ImageUrl = imageUrl;

			var res = await _userManager.UpdateAsync(user);

			if (res.Succeeded)
			{
				if (isEmailUpdate)
					await _authService.VerifyEmailAsync(user.Id, await _userManager.GenerateEmailConfirmationTokenAsync(user));

				ViewBag.IsSucess = true;
				ViewBag.IsFaild = false;

				return View(model);
			}

			ViewBag.IsFaild = true;
			ViewBag.IsSucess = false;

			return View(model);
		}


		public async Task<IActionResult> Edit()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return NotFound();

			return View(user);
		}

	}
}
