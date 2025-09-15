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

			using var dek = Aes.Create();
			dek.KeySize = 256;
			dek.GenerateKey();

			byte[] iv = new byte[12]; // IV ثابت الحجم لـ AES-GCM
			RandomNumberGenerator.Fill(iv);

			byte[] tag = new byte[16];
			byte[] buffer = new byte[4 * 1024 * 1024]; // 4MB buffer

			using var aesGcm = new AesGcm(dek.Key);
			using var inputStream = file.OpenReadStream();
			using var outputStream = File.Create(outputFilePath);

			int bytesRead;
			while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
			{
				byte[] cipherChunk = new byte[bytesRead];
				aesGcm.Encrypt(iv, buffer.AsSpan(0, bytesRead), cipherChunk, tag);
				await outputStream.WriteAsync(cipherChunk.AsMemory(0, bytesRead));
			}

			byte[] encryptedDEK = clientPublicKey.Encrypt(dek.Key, RSAEncryptionPadding.OaepSHA256);

			return (outputFilePath, iv, tag, encryptedDEK);
		}



	}
}
