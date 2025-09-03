using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Interface
{
	public interface IFileService
	{
		Task<string> AddFile(IFormFile file, string FolderName);
		Task<bool> DeleteFile(string fileName);
		Task<byte[]> GetFile(string fileName);
		Task<bool> FileExists(string fileName);
		Task<IEnumerable<string>> GetFilesInFolder(string folderName);
		Task<string> UpdateFile(IFormFile file, string existingFileName, string folderName);
	}
}
