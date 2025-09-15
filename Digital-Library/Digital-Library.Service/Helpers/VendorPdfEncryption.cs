using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace Digital_Library.Core.Services
{
	public class VendorPdfEncryption
	{
		private readonly byte[] _encryptionKey;
		private const int BufferSize = 4 * 1024 * 1024; // 4 ميجا لكل Chunk

		public VendorPdfEncryption(string encryptionKey)
		{
			if (string.IsNullOrWhiteSpace(encryptionKey))
				throw new ArgumentException("Encryption key cannot be null or empty.");

			var keyBytes = System.Text.Encoding.UTF8.GetBytes(encryptionKey);
			if (keyBytes.Length < 32)
				throw new ArgumentException("Encryption key must be at least 32 bytes.");

			_encryptionKey = keyBytes.Take(32).ToArray();
		}

		public async Task<(string EncryptedFilePath, byte[] IV, byte[] Tag)> EncryptFileAsync(IFormFile file, string outputFilePath)
		{
			if (file == null || file.Length == 0)
				throw new ArgumentException("File is null or empty", nameof(file));

			byte[] iv = new byte[12];
			RandomNumberGenerator.Fill(iv);
			byte[] fileData;

			using (var ms = new MemoryStream())
			{
				await file.CopyToAsync(ms);
				fileData = ms.ToArray();
			}

			byte[] ciphertext = new byte[fileData.Length];
			byte[] tag = new byte[16];

			using var aesGcm = new AesGcm(_encryptionKey);
			aesGcm.Encrypt(iv, fileData, ciphertext, tag);

			await File.WriteAllBytesAsync(outputFilePath, ciphertext);
			return (outputFilePath, iv, tag);
		}

		public async Task DecryptFileAsync(string encryptedFilePath, string outputFilePath, byte[] iv, byte[] tag)
		{
			byte[] encryptedData = await File.ReadAllBytesAsync(encryptedFilePath);
			byte[] decryptedData = new byte[encryptedData.Length];

			using var aesGcm = new AesGcm(_encryptionKey);
			aesGcm.Decrypt(iv, encryptedData, tag, decryptedData);

			await File.WriteAllBytesAsync(outputFilePath, decryptedData);
		}

	}
}
