using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.PdfViewer.ViewModel
{
	public class JwtResponse
	{
		public string Token { get; set; }
		public DateTime Expiration { get; set; }
		public string UserId { get; set; }
		public string Email { get; set; }
		public List<string> Roles { get; set; }
	}
}
