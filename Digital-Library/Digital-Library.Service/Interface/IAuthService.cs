namespace Digital_Library.Service.Interface;
public interface IAuthService
{
    Task SignOut();
    Task SignIn(string email, string password);
    Task<bool> SignUp(string name, string email, string password);
    Task ForgetPassword(string email);
    Task VerifyEmail(string userId, string token);
    Task<bool> ChangePassword(string userId,string oldPassword, string newPassword);
    Task<bool> ResetPassword(string userId, string token, string newPassword);
}
