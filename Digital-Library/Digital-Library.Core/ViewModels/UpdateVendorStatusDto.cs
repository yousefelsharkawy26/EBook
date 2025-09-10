using Digital_Library.Core.Enums;

namespace Digital_Library.Core.ViewModels;
public class UpdateVendorStatusDto
{
    public string VendorId { get; set; }
    public VendorStatus NewStatus { get; set; }
}
