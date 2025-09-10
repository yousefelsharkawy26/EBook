namespace Digital_Library.Core.ViewModels;

// en Core/ViewModels/DashboardViewModel.cs
public class DashboardViewModel
{
    // Para las tarjetas de KPI (Key Performance Indicators)
    public decimal TotalRevenue { get; set; }
    public int NewUsersToday { get; set; }
    public int PendingVendorsCount { get; set; }
    public int TotalBooksCount { get; set; }

    // Para las tablas de actividad reciente
    public List<RecentSaleViewModel> RecentSales { get; set; } = new List<RecentSaleViewModel>();
    public List<PendingVendorViewModel> PendingVendors { get; set; } = new List<PendingVendorViewModel>();
}
