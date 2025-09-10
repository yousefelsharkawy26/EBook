// Core/ViewModels/OrderSummaryViewModel.cs
namespace Digital_Library.Core.ViewModels;

// نموذج مساعد لعرض ملخص الطلب
public class OrderSummaryViewModel
{
    public string OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal OrderTotal { get; set; }
    public string OrderStatus { get; set; }
}
