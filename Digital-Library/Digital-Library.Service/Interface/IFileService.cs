using Digital_Library.Core.Constant;
using Digital_Library.Core.Enums;
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
		Task<string> AddFile(IFormFile file, string folderName, StorageType storageType = StorageType.Public);
		Task<bool> DeleteFile(string fileName,StorageType storageType = StorageType.Public);
		Task<string> UpdateFile(IFormFile file, string existingFileName, StorageType storageType = StorageType.Public);
		Task<string> AddBytes(byte[] fileBytes, string fileName, string folderName, StorageType storageType = StorageType.Public);
		Task<string> GetFolderPath(string folder);
	}
}
