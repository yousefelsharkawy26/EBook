using Digital_Library.PdfViewer.Helpers;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Digital_Library.PdfViewer.Services
{
    public class KeyManagementService : IKeyManagementService
    {
        private readonly IAuthService _authService;
        private readonly string _privateKeyPath;

        public KeyManagementService(IAuthService authService)
        {
            _authService = authService;
            _privateKeyPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "private.key");
        }

        public async Task<(RSA rsa, string publicKeyBase64)> LoadOrCreateKeysAsync()
        {
            RSA rsa = RSA.Create();
            string publicKeyBase64;
            string privateKeyBase64;

            if (!File.Exists(_privateKeyPath))
            {
                (publicKeyBase64, privateKeyBase64) = KeyHelper.GenerateKeyPair();

                // Register the new public key with the server
                await _authService.RegisterPublicKeyAsync(publicKeyBase64);

                // Protect and save the private key locally
                var encryptedKey = ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(privateKeyBase64),
                    null,
                    DataProtectionScope.CurrentUser);
                await File.WriteAllBytesAsync(_privateKeyPath, encryptedKey);
            }
            else
            {
                var encryptedKey = await File.ReadAllBytesAsync(_privateKeyPath);
                var privateKeyBytes = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);
                privateKeyBase64 = Encoding.UTF8.GetString(privateKeyBytes);
            }

            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
            publicKeyBase64 = Convert.ToBase64String(rsa.ExportRSAPublicKey());

            return (rsa, publicKeyBase64);
        }
    }
}