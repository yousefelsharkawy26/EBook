using System.Security.Cryptography;

namespace Digital_Library.PdfViewer.Services;
public interface IKeyManagementService
{
    // Returns the RSA provider and the public key
    Task<(RSA rsa, string publicKeyBase64)> LoadOrCreateKeysAsync();
}
