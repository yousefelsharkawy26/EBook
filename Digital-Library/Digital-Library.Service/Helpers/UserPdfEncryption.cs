using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Digital_Library.Service.Helpers
{
	public class UserPdfEncryptionService
	{
		public async Task<(string EncryptedFilePath, byte[] IV, byte[] Tag, byte[] EncryptedDEK)>
					EncryptFileAsync(IFormFile file, RSA clientPublicKey, string outputFilePath)
		{
			if (file == null || file.Length == 0)
				throw new ArgumentException("File is null or empty", nameof(file));

			// إنشاء DEK
			using var dek = Aes.Create();
			dek.KeySize = 256;
			dek.GenerateKey();

			byte[] iv = new byte[12];
			RandomNumberGenerator.Fill(iv);

			byte[] fileBytes;
			using (var ms = new MemoryStream())
			{
				await file.CopyToAsync(ms);
				fileBytes = ms.ToArray();
			}

			byte[] cipherText = new byte[fileBytes.Length];
			byte[] tag = new byte[16];

			using (var aesGcm = new AesGcm(dek.Key))
			{
				aesGcm.Encrypt(iv, fileBytes, cipherText, tag);
			}

			await File.WriteAllBytesAsync(outputFilePath, cipherText);

			byte[] encryptedDEK = clientPublicKey.Encrypt(dek.Key, RSAEncryptionPadding.OaepSHA256);

			return (outputFilePath, iv, tag, encryptedDEK);
		}




	}
}
