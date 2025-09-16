using Digital_Library.PdfViewer.Enum;
using Digital_Library.PdfViewer.Helpers;
using Digital_Library.PdfViewer.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace Digital_Library.PdfViewer.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly UserSession _userSession;
        public BookService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("E-Book Client");
            // The token should be set on the HttpClient instance when the service is created
            // This is typically handled by a factory or dependency injection
            _userSession = UserSession.Instance;

            if (_userSession.JwtToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _userSession.JwtToken);
            }
        }

        public async Task<PagedResult<UserBookDto>> GetMyBooksAsync(int page, int pageSize)
        {
            // Throws HttpRequestException on failure, which the ViewModel will catch
            var result = await _httpClient.GetFromJsonAsync<PagedResult<UserBookDto>>(
                $"MyBooks?page={page}&pageSize={pageSize}");

            if (result == null)
            {
                throw new InvalidOperationException("Failed to retrieve books from the server.");
            }
            return result;
        }

        public async Task<(byte[] DecryptedPdf, string Email, FormatType Type)> GetAndDecryptPdfAsync(string bookId)
        {
            var response = await _httpClient.GetFromJsonAsync<EncryptedPdfViewModel>(
                $"ShowPdf/{bookId}");

            if (response == null)
            {
                throw new InvalidOperationException("Failed to load PDF data from the server.");
            }

            var decryptedBytes = DecryptPdf(
                response.EncryptedFile,
                response.EncryptedDEK,
                response.IV,
                response.Tag);

            var type = response.type switch
            {
                "PDF" => FormatType.PDF,
                "Borrowing" => FormatType.Borrowing,
                _ => FormatType.Physical,
            };

            return (decryptedBytes, response.Email, type);
        }

        private byte[] DecryptPdf(byte[] encryptedFile, byte[] encryptedDEK, byte[] iv, byte[] tag)
        {
            if (_userSession.Rsa == null)
                throw new InvalidOperationException("User RSA keys are not loaded.");

            byte[] dek = _userSession.Rsa.Decrypt(encryptedDEK, RSAEncryptionPadding.OaepSHA256);

            byte[] decrypted = new byte[encryptedFile.Length];
            using var aesGcm = new AesGcm(dek);
            aesGcm.Decrypt(iv, encryptedFile, tag, decrypted);

            return decrypted;
        }
    }
}