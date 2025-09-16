using Digital_Library.PdfViewer.Helper;
using Digital_Library.PdfViewer.ViewModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Digital_Library.PdfViewer
{
	public partial class LoginWindow : Window
	{
		private readonly HttpClient _httpClient;

		public LoginWindow()
		{
			InitializeComponent();
			_httpClient = new HttpClient();
		}

		private async void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			var email = EmailBox.Text;
			var password = PasswordBox.Password;
			var loginRequest = new { Email = email, Password = password };

			try
			{
				var response = await _httpClient.PostAsJsonAsync("https://zzaki213-001-site1.stempurl.com/api/Client/login", loginRequest);
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<ApiResponse<JwtResponse>>();
					if (result?.Success == true)
					{
						// حفظ الـ JWT في الجلسة
						UserSession.Instance.JwtToken = result.Data.Token;
						UserSession.Instance.TokenExpiration = result.Data.Expiration;
						_httpClient.DefaultRequestHeaders.Authorization =
										new AuthenticationHeaderValue("Bearer", UserSession.Instance.JwtToken);

						var privateKeyPath = Path.Combine(
										Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
										"private.key");

						RSA rsa;
						string publicKeyBase64;

						if (!File.Exists(privateKeyPath))
						{
							
							(publicKeyBase64, string privateKeyBase64) = KeyHelper.GenerateKeyPair();

					
							var publicKeyRequest = new { PublicKey = publicKeyBase64 };
							var keyResponse = await _httpClient.PostAsJsonAsync(
											"https://zzaki213-001-site1.stempurl.com/api/Client/register-public-key",
											publicKeyRequest);

							if (!keyResponse.IsSuccessStatusCode)
							{
								MessageBox.Show("Login successful, but failed to register public key.");
								return;
							}

							var encryptedKey = ProtectedData.Protect(
											Encoding.UTF8.GetBytes(privateKeyBase64),
											null,
											DataProtectionScope.CurrentUser);
							File.WriteAllBytes(privateKeyPath, encryptedKey);

							rsa = RSA.Create();
							rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
						}
						else
						{
							var encryptedKey = File.ReadAllBytes(privateKeyPath);
							var privateKeyBytes = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);
							var privateKeyBase64 = Encoding.UTF8.GetString(privateKeyBytes);

							rsa = RSA.Create();
							rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);

							publicKeyBase64 = Convert.ToBase64String(rsa.ExportRSAPublicKey());
						}

						UserSession.Instance.SetKeys(rsa, publicKeyBase64);

						var MyBooksWindow = new MyBooksWindow();
						MyBooksWindow.Show();
						this.Close();
					}
					else
					{
						MessageBox.Show(result?.Message ?? "Login failed");
					}
				}
				else
				{
					MessageBox.Show("Server error: " + response.StatusCode);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message);
			}
		}
	}
}
