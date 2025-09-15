using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.PdfViewer.Helper
{
	public class UserSession
	{
		private static UserSession? _instance;
		public static UserSession Instance => _instance ??= new UserSession();

		private UserSession() { }

		public string JwtToken { get; set; } = string.Empty;
		public DateTime TokenExpiration { get; set; }

		public RSA? Rsa { get; private set; }
		public string? PublicKeyBase64 { get; private set; }

		public void SetKeys(RSA rsa, string publicKeyBase64)
		{
			Rsa = rsa;
			PublicKeyBase64 = publicKeyBase64;
		}

		public bool IsTokenValid =>
						!string.IsNullOrEmpty(JwtToken) && DateTime.UtcNow < TokenExpiration;
	}
}
