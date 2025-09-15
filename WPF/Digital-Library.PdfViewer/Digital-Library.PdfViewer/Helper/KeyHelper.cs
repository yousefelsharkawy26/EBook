using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.PdfViewer.Helper
{
	public class KeyHelper
	{
		public static (string publicKey, string privateKey) GenerateKeyPair()
		{
			using var rsa = RSA.Create(2048);

			var publicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());


			var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

			return (publicKey, privateKey);
		}

	}
}

