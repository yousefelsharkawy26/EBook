using Digital_Library.PdfViewer.Models;

namespace Digital_Library.PdfViewer.Services;

public interface IAuthService
{
    string UserId { get; }
    Task<JwtResponse> LoginAsync(string email, string password);
    Task RegisterPublicKeyAsync(string publicKey);
}
