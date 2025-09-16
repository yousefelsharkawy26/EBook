using Digital_Library.PdfViewer.Helpers;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Digital_Library.PdfViewer.Services
{
    public class KeyManagementService : IKeyManagementService
    {
        private readonly IAuthService _authService;

        public KeyManagementService(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<(RSA rsa, string publicKeyBase64)> LoadOrCreateKeysAsync()
        {
            string privateKeyFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _authService.UserId);
            if (!Directory.Exists(privateKeyFolder))
                Directory.CreateDirectory(privateKeyFolder);

            string privateKeyPath = Path.Combine(privateKeyFolder, "private.key");

            RSA rsa = RSA.Create();
            string publicKeyBase64;
            string privateKeyBase64;

            if (!File.Exists(privateKeyPath))
            {
                (publicKeyBase64, privateKeyBase64) = KeyHelper.GenerateKeyPair();

                // Register the new public key with the server
                await _authService.RegisterPublicKeyAsync(publicKeyBase64);

                // Protect and save the private key locally
                var encryptedKey = ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(privateKeyBase64),
                    null,
                    DataProtectionScope.CurrentUser);
                await File.WriteAllBytesAsync(privateKeyPath, encryptedKey);
            }
            else
            {
                var encryptedKey = await File.ReadAllBytesAsync(privateKeyPath);
                var privateKeyBytes = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);
                privateKeyBase64 = Encoding.UTF8.GetString(privateKeyBytes);
            }

            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
            publicKeyBase64 = Convert.ToBase64String(rsa.ExportRSAPublicKey());

            return (rsa, publicKeyBase64);
        }
    }
}