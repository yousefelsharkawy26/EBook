namespace Digital_Library.Core.ViewModels.Requests;
public class ChangePasswordRequest
{
    public string UserId { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}
