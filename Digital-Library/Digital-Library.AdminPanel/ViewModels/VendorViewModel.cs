namespace Digital_Library.AdminPanel.ViewModels;

public class VendorViewModel
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string LibraryName { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string ContactNumber { get; set; }
    public decimal WalletBalance { get; set; }
    public string Status { get; set; }
    public string RejectionReason { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    // ====> الخاصية الجديدة التي تمت إضافتها <====
    // هذه قائمة بسيطة من النصوص، لا تسبب أي حلقات
    public List<string> IdentityImageUrls { get; set; } = new List<string>();
    public string ImageBaseUrl { get; set; }
}
