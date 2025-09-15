using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels
{
	public class FileEncDetail
	{
		public byte[]? EncryptedDEK { get; set; }

		public byte[]? IV { get; set; }

		public byte[]? Tag { get; set; }

		public string? filePath { get; set; }

	}
}
