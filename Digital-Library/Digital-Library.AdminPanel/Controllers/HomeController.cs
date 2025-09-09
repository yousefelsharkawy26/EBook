using Digital_Library.AdminPanel.Models;
using Digital_Library.AdminPanel.ViewModels;
using Digital_Library.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Digital_Library.AdminPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, 
                              UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Forms()
        {
            return View();
        }
        public IActionResult Tables()
        {
            return View();
        }
        public IActionResult Charts()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> GetUserGrowthChartData()
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

            return Json(chartData); // إرجاع البيانات كـ JSON
        }

        public IActionResult Defult()
        {
            return Json("Hello");
        }
    }
}
