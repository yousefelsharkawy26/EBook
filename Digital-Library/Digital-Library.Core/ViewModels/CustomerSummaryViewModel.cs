// Core/ViewModels/CustomerSummaryViewModel.cs
namespace Digital_Library.Core.ViewModels;
public class CustomerSummaryViewModel
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int TotalOrders { get; set; }
    public bool IsActive { get; set; } // لمعرفة إذا كان الحساب محظورًا
}
