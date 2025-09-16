using Digital_Library.Core.Constant;
using Digital_Library.Core.ViewModels;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Digital_Library.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ClientController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly IBookService bookService;
		private readonly IFileService fileService;

		public ClientController(IAuthService authService, IBookService bookService,IFileService fileService)
		{
			_authService = authService;
			this.bookService = bookService;
			this.fileService = fileService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var response = await _authService.SignInWithJwtAsync(request.Email, request.Password);

			if (!response.Success)
				return Unauthorized(response);

			return Ok(response); 
		}
		[HttpGet("MyBooks")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> MyBooks(int page = 1, int pageSize = 10)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(new { Message = "User not found or not authenticated" });

			var result = await bookService.GetUserBooksAsync(userId, page, pageSize);

			return Ok(result); 
		}
		[HttpPost("register-public-key")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> RegisterPublicKey([FromBody] PublicKeyRequest request)
		{
			if (string.IsNullOrEmpty(request.PublicKey))
				return BadRequest("Public key is required");

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null)
			{
				return Unauthorized();
			}


			var response = await _authService.SaveUserPublicKeyAsync(userId, request.PublicKey);

			if (!response.Success)
				return BadRequest();

			return Ok();
		}

		[HttpGet("ShowPdf/{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> ShowPdf(string id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized();

			var result = await bookService.ShowPdf(id, userId);

			if (!result.Success)
				return BadRequest(result.Message);

			var data = result.Data as FileEncDetail;
			var folderPath = await fileService.GetFolderPath(FileFoldersName.UsersBooksPdf);
			data.filePath = Path.Combine(folderPath,Path.GetFileName( data.filePath));
			if (data == null || !System.IO.File.Exists(data.filePath))
				return NotFound("Encrypted PDF not found.");

			var fileBytes = await System.IO.File.ReadAllBytesAsync(data.filePath);

			var viewModel = new EncryptedPdfViewModel
			{
				EncryptedFile = fileBytes,
				IV = data.IV,
				Tag = data.Tag,
				EncryptedDEK = data.EncryptedDEK,
				FileName = Path.GetFileName(data.filePath)
				,Email=data.Email,
				type=data.type
			};

			return Ok(viewModel);
		}



	}
}
