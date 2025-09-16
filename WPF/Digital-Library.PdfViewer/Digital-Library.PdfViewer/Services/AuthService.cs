using Digital_Library.PdfViewer.Helpers;
using Digital_Library.PdfViewer.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Digital_Library.PdfViewer.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    public string UserId { get; private set; }
    public AuthService(IHttpClientFactory httpClient)
    {
        _httpClient = httpClient.CreateClient("E-Book Client");
    }

    public async Task<JwtResponse> LoginAsync(string email, string password)
    {
        var loginRequest = new { Email = email, Password = password };
        var response = await _httpClient.PostAsJsonAsync("login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            // Throw a specific exception for better error handling in the ViewModel
            throw new HttpRequestException($"Server error: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<JwtResponse>>();
        if (result?.Success != true)
        {
            throw new Exception(result?.Message ?? "Login failed due to an unknown error.");
        }

        // Update the HttpClient for subsequent requests
        UserSession.Instance.JwtToken = result.Data.Token;
        UserSession.Instance.TokenExpiration = result.Data.Expiration;
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", UserSession.Instance.JwtToken);

        UserId = result.Data.UserId;

        return result.Data;
    }

    public async Task RegisterPublicKeyAsync(string publicKey)
    {
        var publicKeyRequest = new { PublicKey = publicKey };
        var response = await _httpClient.PostAsJsonAsync($"register-public-key", publicKeyRequest);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Failed to register public key with the server.");
        }
    }
}
