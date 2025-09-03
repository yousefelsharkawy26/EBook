using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Implementation
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Hosting;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	public class FileService : IFileService
	{
		private readonly IWebHostEnvironment _webHostEnvironment;

		public FileService(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<string> AddFile(IFormFile file, string folderName)
		{
			if (file == null || file.Length == 0)
				return null;

			folderName = folderName.Trim().Replace("\\", "/");
			string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
			string fullFilePath = Path.Combine(folderPath, uniqueFileName);

			using (var stream = new FileStream(fullFilePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			return Path.Combine(folderName, uniqueFileName).Replace("\\", "/");
		}

		public async Task<bool> DeleteFile(string fileName)
		{
			string filePath =  Path.Combine(_webHostEnvironment.WebRootPath, fileName);

			if (File.Exists(filePath))
			{
				File.Delete(filePath);
				return true;
			}

			return false;
		}

		public async Task<byte[]> GetFile(string fileName)
		{
			string filePath = Path.Combine(_webHostEnvironment.WebRootPath, fileName);

			if (File.Exists(filePath))
			{
				return await File.ReadAllBytesAsync(filePath);
			}

			return null;
		}

		public async Task<bool> FileExists(string fileName)
		{
			string filePath = Path.Combine(_webHostEnvironment.WebRootPath, fileName);
			return File.Exists(filePath);
		}

		public async Task<IEnumerable<string>> GetFilesInFolder(string folderName)
		{
			string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);

			if (!Directory.Exists(folderPath))
				return Enumerable.Empty<string>();

			var files = Directory.GetFiles(folderPath)
																								.Select(f => Path.Combine(folderName, Path.GetFileName(f)).Replace("\\", "/"));

			return files;
		}

		public async Task<string> UpdateFile(IFormFile file, string existingFileName, string folderName)
		{
			await DeleteFile(existingFileName);

			return await AddFile(file, folderName);
		}
	}

}
