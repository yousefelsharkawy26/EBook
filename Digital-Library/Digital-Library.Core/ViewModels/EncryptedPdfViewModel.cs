using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels
{
	public class EncryptedPdfViewModel
	{
		public byte[] EncryptedFile { get; set; }   
		public byte[] IV { get; set; }             
		public byte[] Tag { get; set; }             
		public byte[] EncryptedDEK { get; set; }    
		public string FileName { get; set; }        
	}
}
