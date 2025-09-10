// Core/ViewModels/CustomerDetailsViewModel.cs
namespace Digital_Library.Core.ViewModels;

// في Core/ViewModels/CustomerDetailsViewModel.cs
public class CustomerDetailsViewModel
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool IsActive { get; set; }

    // قائمة بآخر الطلبات الخاصة بالعميل
    public List<OrderSummaryViewModel> RecentOrders { get; set; } = new List<OrderSummaryViewModel>();
}
