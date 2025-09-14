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
	using Digital_Library.Core.Constant;
	using Digital_Library.Core.Enums;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Hosting;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	public class FileService : IFileService
	{
		private readonly IWebHostEnvironment _env;

		public FileService(IWebHostEnvironment env)
		{
			_env = env;
		}
		public async Task<string> GetFolderPath(string folder)
		{
			string folderPath = Path.Combine(_env.ContentRootPath, "Files", folder);


			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			return folderPath;
		}


		public async Task<string> AddFile(IFormFile file, string folderName, StorageType storageType = StorageType.Public)
		{
			if (file == null || file.Length == 0)
				return null;

			folderName = folderName.Trim().Replace("\\", "/");

			string basePath = storageType == StorageType.Public
							? Path.Combine(_env.WebRootPath, folderName)
							: Path.Combine(_env.ContentRootPath, "Files", folderName);

			if (!Directory.Exists(basePath))
				Directory.CreateDirectory(basePath);

			string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
			string fullFilePath = Path.Combine(basePath, uniqueFileName);

			using (var stream = new FileStream(fullFilePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
			return Path.Combine(folderName, uniqueFileName).Replace("\\", "/");
		}

		public async Task<bool> DeleteFile(string relativePath, StorageType storageType = StorageType.Public)
		{
			if (string.IsNullOrEmpty(relativePath))
				return false;

			string basePath = storageType == StorageType.Public
							? _env.WebRootPath
							: Path.Combine(_env.ContentRootPath, "Files");

			string filePath = Path.Combine(basePath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

			if (File.Exists(filePath))
			{
				File.Delete(filePath);
				return true;
			}

			await Task.CompletedTask;
			return false;
		}

		public async Task<string> UpdateFile(IFormFile file, string existingRelativePath, StorageType storageType = StorageType.Public)
		{
			if (!string.IsNullOrEmpty(existingRelativePath))
				await DeleteFile(existingRelativePath, storageType);

			string folderName = Path.GetDirectoryName(existingRelativePath)?.Replace("\\", "/") ?? "";
			return await AddFile(file, folderName, storageType);
		}
		public async Task<string> AddBytes(byte[] fileBytes, string fileName, string folderName, StorageType storageType = StorageType.Public)
		{
			if (fileBytes == null || fileBytes.Length == 0)
				return null;

			folderName = folderName.Trim().Replace("\\", "/");

			string basePath = storageType == StorageType.Public
																			? Path.Combine(_env.WebRootPath, folderName)
																			: Path.Combine(_env.ContentRootPath, "Files", folderName);

			if (!Directory.Exists(basePath))
				Directory.CreateDirectory(basePath);

			string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
			string fullFilePath = Path.Combine(basePath, uniqueFileName);

			await File.WriteAllBytesAsync(fullFilePath, fileBytes);

			return Path.Combine(folderName, uniqueFileName).Replace("\\", "/");
		}
	}

}
