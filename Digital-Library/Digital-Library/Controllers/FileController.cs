using Digital_Library.Core.Constant;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Digital_Library.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FileController : ControllerBase
	{
		private readonly IFileService fileService;
		private readonly IVendorService vendorService;
		private readonly IWebHostEnvironment env;

		public FileController(IFileService fileService,IVendorService vendorService, IWebHostEnvironment env)
		{
			this.fileService = fileService;
			this.vendorService = vendorService;
			this.env = env;
		}
		[HttpPost("upload")]
		public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string oldNameFile)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file uploaded.");
			}

			if (string.IsNullOrWhiteSpace(FileFoldersName.BooksImageCover))
			{
				return BadRequest("Folder name is required.");
			}

			var filePath = await fileService.UpdateFile(file, oldNameFile);

			if (filePath == null)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading file.");
			}

			return Ok(new { FilePath = filePath });
		}
		[HttpGet("photo")]
		public async Task<IActionResult> GetPrivatePhoto([FromQuery] string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return BadRequest("File name is required.");

			var filePath = Path.Combine(env.ContentRootPath, "Files",fileName);

			if (!System.IO.File.Exists(filePath))
				return NotFound();

			var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			var contentType = "image/jpeg"; 
			return File(stream, contentType, fileName);
		}


	}
}
