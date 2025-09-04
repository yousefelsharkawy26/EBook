using Digital_Library.Core.ViewModels.Responses;

namespace Digital_Library.Service.Interface
{
	public interface IAuthService
	{
		Task<Response> SignInAsync(string email, string password);
		Task<Response> SignOutAsync();
		Task<Response> SignUpAsync(string name, string email, string password);
		Task<Response> ForgetPasswordAsync(string email);
		Task<Response> ResetPasswordAsync(string userId, string token, string newPassword);
		Task<Response> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
		Task<Response> VerifyEmailAsync(string userId, string token);
		Task<Response> ChangeEmailAsync(string userId, string newEmail);
	}

}