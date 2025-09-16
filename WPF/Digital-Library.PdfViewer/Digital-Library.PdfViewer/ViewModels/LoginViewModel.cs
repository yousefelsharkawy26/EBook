using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Digital_Library.PdfViewer.Helpers;
using Digital_Library.PdfViewer.Services;
using System.Security;
using System.Windows.Controls;

namespace Digital_Library.PdfViewer.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IKeyManagementService _keyManagementService;

        [ObservableProperty] private string _email;


        // Use SecureString for password in ViewModel for better security
        [ObservableProperty] private SecureString _securePassword;

        [ObservableProperty] private string _errorMessage;

        // Event to signal the View to perform navigation
        public event EventHandler LoginSucceeded;

        public LoginViewModel(IAuthService authService, 
                              IKeyManagementService keyManagementService)
        {
            _authService = authService;
            _keyManagementService = keyManagementService;
        }

        //public bool CanLogin() => !string.IsNullOrWhiteSpace(Email) && SecurePassword?.Length > 0;

        [RelayCommand]
        private async Task LoginAsync(PasswordBox passwordBox)
        {
            ErrorMessage = string.Empty;
            try
            {
                // Note: We need to convert SecureString to a string for the API call.
                // This is the point where the password temporarily exists in memory.
                var password = passwordBox.Password;

                // 1. Authenticate and get JWT
                var auth = await _authService.LoginAsync(Email, password);
                
                // 2. Load or create cryptographic keys
                var (rsa, publicKey) = await _keyManagementService.LoadOrCreateKeysAsync();

                // 3. Store keys in the user session
                UserSession.Instance.SetKeys(rsa, publicKey);

                // 4. Signal success to the View
                OnLoginSucceeded();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
        }

        protected virtual void OnLoginSucceeded()
        {
            LoginSucceeded?.Invoke(this, EventArgs.Empty);
        }
    }
}