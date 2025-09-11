using Digital_Library.Core.Enum;
using Digital_Library.Core.Enums;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Identity;
// Infrastructure/Services/DashboardService.cs
using Microsoft.EntityFrameworkCore;

namespace Digital_Library.Service.Implementation;
public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;

    public DashboardService(UserManager<User> userManager,
                            IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var today = DateTime.UtcNow.Date;

        var orderHeaders = await _unitOfWork.OrderHeaders
            .GetManyAsync(o => o.Status == Status.Complete);

        var totalRevenue = orderHeaders.Sum(o => o.TotalAmount);

        var newUsersToday = await _userManager.Users
            .CountAsync(u => u.CreationDate.Date == today);

        var pendingVendors = await _unitOfWork.Vendors
            .GetAllQuery()
            .Include(v => v.User)
            .Where(v => v.Status == VendorStatus.Pending)
            .OrderByDescending(v => v.SubmittedAt)
            .Take(5) // Solo los 5 más recientes
            .Select(v => new PendingVendorViewModel
            {
                VendorId = v.Id,
                VendorName = v.User.FullName,
                LibraryName = v.LibraryName
            }).ToListAsync();

        var recentSales = await _unitOfWork.Orders
            .GetAllQuery()
            .Include(o => o.User)
            .Include(o => o.OrderHeaders)
            .OrderByDescending(o => o.OrderDate)
            .Take(5) // Solo las 5 más recientes
            .Select(o => new RecentSaleViewModel
            {
                CustomerName = o.User.FullName,
                SaleDate = o.OrderDate,
                Amount = o.OrderHeaders.Sum(u => u.TotalAmount),
                Status = o.OrderHeaders.All(u => u.Status == Status.Pending) ? "Pending" :
                              o.OrderHeaders.All(u => u.Status == Status.Complete) ? "Completed" :
                              o.OrderHeaders.All(u => u.Status == Status.Cancelled) ? "Cancelled" : "In Progress",
            }).ToListAsync();

        var totalBooksCount = await _unitOfWork.Books.CountAsync();

        var viewModel = new DashboardViewModel
        {
            TotalRevenue = totalRevenue,
            NewUsersToday = newUsersToday,
            PendingVendorsCount = await _unitOfWork.Vendors.CountAsync(v => v.Status == VendorStatus.Pending),
            TotalBooksCount = totalBooksCount,
            RecentSales = recentSales,
            PendingVendors = pendingVendors
        };

        return viewModel;
    }

    public async Task<ChartDataViewModel> GetUserGrowthChartDataAsync(int days = 30)
    {
        var chartData = new ChartDataViewModel();
        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-(days - 1));

        var dailyNewUsers = await _userManager.Users
            .Where(u => u.CreationDate.Date >= startDate)
            .GroupBy(u => u.CreationDate.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();

        for (int i = 0; i < days; i++)
        {
            var currentDate = startDate.AddDays(i);
            chartData.Labels.Add(currentDate.ToString("MMM dd"));
            chartData.Data.Add(dailyNewUsers.FirstOrDefault(d => d.Date == currentDate)?.Count ?? 0);
        }

        return chartData;
    }

    public async Task<ChartDataViewModel> GetSalesChartDataAsync(int days = 30)
    {
        var chartData = new ChartDataViewModel();
        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-29); // آخر 30 يومًا (اليوم + 29 يومًا قبله)

        // 1. احصل على إجمالي عدد المستخدمين قبل فترة الـ 30 يومًا
        var initialUserCount = await _userManager.Users
                                    .CountAsync(u => u.CreationDate.Date < startDate);

        // 2. احصل على عدد المستخدمين الجدد لكل يوم في آخر 30 يومًا
        var dailyNewUsers = await _userManager.Users
            .Where(u => u.CreationDate.Date >= startDate && u.CreationDate.Date <= today)
            .GroupBy(u => u.CreationDate.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Date, x => x.Count);

        // 3. قم بإنشاء بيانات الرسم البياني بشكل تراكمي
        var cumulativeCount = initialUserCount;
        for (int i = 0; i < 30; i++)
        {
            var currentDate = startDate.AddDays(i);

            // أضف التاريخ إلى الـ Labels
            chartData.Labels.Add(currentDate.ToString("MMM dd")); // مثال: "May 23"

            // إذا كان هناك مستخدمون جدد في هذا اليوم، أضفهم إلى العدد التراكمي
            if (dailyNewUsers.ContainsKey(currentDate))
            {
                cumulativeCount += dailyNewUsers[currentDate];
            }

            // أضف العدد التراكمي لهذا اليوم إلى بيانات الرسم البياني
            chartData.Data.Add(cumulativeCount);
        }

        return chartData;
    }
}
