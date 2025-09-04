using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.ViewModels.Responses
{
    public class VendorDashboardResponse
    {
        public string VendorId { get; set; }
        public string LibraryName { get; set; }
        public decimal WalletBalance { get; set; }
        public int TotalBooks { get; set; }
        public int TotalBooksSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
