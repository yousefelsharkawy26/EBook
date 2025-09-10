using Digital_Library.AdminPanel.Models;
using Digital_Library.AdminPanel.ViewModels;
using Digital_Library.Core.Models;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Digital_Library.AdminPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IVendorService _vendorService;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger,
                              UserManager<User> userManager,
                              IVendorService vendorService,
                              IConfiguration config)
        {
            _logger = logger;
            _userManager = userManager;
            _vendorService = vendorService;
            _config = config;
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

        public async Task<IActionResult> NewVendors()
        {
            var vendors = await _vendorService.GetVendorsAsync();

            var vendorViewModels = vendors.Select(v => new VendorViewModel
            {
                Id = v.Id,
                FullName = v.User?.FullName,
                Email = v.User?.Email,
                LibraryName = v.LibraryName,
                City = v.City,
                State = v.State,
                ZipCode = v.ZipCode,
                ContactNumber = v.ContactNumber,
                WalletBalance = v.WalletBalance,
                Status = v.Status.ToString(),
                SubmittedAt = v.SubmittedAt,
                ReviewedAt = v.ReviewedAt,
                ImageBaseUrl = _config["Digital-Library-Url"]!,
                // ====> قم بتعبئة قائمة روابط الصور هنا <====
                // نقوم بتحويل قائمة الكائنات إلى قائمة نصوص (URLs)
                IdentityImageUrls = v.VendorIdentityImagesUrls?.Select(i => i.ImageUrl).ToList() ?? new List<string>()

            }).ToList();

            return View(vendorViewModels);
        }

        [HttpPost("api/Vendors/UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateVendorStatusDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.VendorId))
            {
                return BadRequest("Invalid data.");
            }

            var res = await _vendorService.ChangeStatusAsync(dto.VendorId, dto.NewStatus);

            if (res.Success)
            {
                return Ok(new { message = "Status updated successfully." });
            }

            return NotFound(new { message = "Vendor not found or failed to update." });
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
    }
}
