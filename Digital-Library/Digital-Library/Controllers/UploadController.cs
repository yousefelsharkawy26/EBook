using Digital_Library.Core.Constant;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Digital_Library.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UploadController : ControllerBase
	{
		private readonly IFileService fileService;

		public UploadController(IFileService fileService)
		{
			this.fileService = fileService;
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

			var filePath = await fileService.UpdateFile(file, oldNameFile, FileFoldersName.BooksImageCover);

			if (filePath == null)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading file.");
			}

			return Ok(new { FilePath = filePath });
		}

	}
}
