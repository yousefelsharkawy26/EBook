namespace Digital_Library.Service.Interface;
public interface IAuthService
{
    Task SignOut();
    Task SignIn(string email, string password);
    Task SignUp(string name, string email, string password);
    Task ForgetPassword(string email);
    Task VerifyEmail(string userId);
    Task<bool> ChangePassword(string userId,string oldPassword, string newPassword);
}
