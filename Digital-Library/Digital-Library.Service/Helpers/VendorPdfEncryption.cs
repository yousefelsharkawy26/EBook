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

		/// <summary>
		/// تشفير ملف كبير مباشرة على القرص باستخدام Streaming
		/// </summary>
		public async Task<(string EncryptedFilePath, byte[] IV, byte[] Tag)> EncryptFileAsync(IFormFile file, string outputFilePath)
		{
			if (file == null || file.Length == 0)
				throw new ArgumentException("File is null or empty", nameof(file));

			byte[] iv = new byte[12];
			RandomNumberGenerator.Fill(iv);
			byte[] tag = new byte[16];

			using var inputStream = file.OpenReadStream();
			using var outputStream = File.Create(outputFilePath);

			byte[] buffer = new byte[BufferSize];
			byte[] ciphertext = new byte[BufferSize];

			int bytesRead;
			using var aesGcm = new AesGcm(_encryptionKey);

			while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
			{
				aesGcm.Encrypt(iv, buffer.AsSpan(0, bytesRead), ciphertext.AsSpan(0, bytesRead), tag);
				await outputStream.WriteAsync(ciphertext.AsMemory(0, bytesRead));
			}

			return (outputFilePath, iv, tag);
		}

		/// <summary>
		/// فك تشفير ملف كبير على القرص باستخدام Streaming
		/// </summary>
		public async Task DecryptFileToDiskAsync(string encryptedFilePath, string outputFilePath, byte[] iv, byte[] tag)
		{
			if (!File.Exists(encryptedFilePath))
				throw new FileNotFoundException("Encrypted file not found", encryptedFilePath);

			using var inputStream = File.OpenRead(encryptedFilePath);
			using var outputStream = File.Create(outputFilePath);

			byte[] buffer = new byte[BufferSize];
			byte[] plaintext = new byte[BufferSize];

			int bytesRead;
			using var aesGcm = new AesGcm(_encryptionKey);

			while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
			{
				aesGcm.Decrypt(iv, buffer.AsSpan(0, bytesRead), tag, plaintext.AsSpan(0, bytesRead));
				await outputStream.WriteAsync(plaintext.AsMemory(0, bytesRead));
			}
		}
	}
}
