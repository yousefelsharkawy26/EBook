using System.Security.Cryptography;

namespace Digital_Library.PdfViewer.Helpers;
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

