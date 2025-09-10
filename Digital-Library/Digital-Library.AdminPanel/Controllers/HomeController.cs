using Digital_Library.AdminPanel.Models;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
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
        private readonly IVendorService _vendorService;
        private readonly IDashboardService _dashboardService;

        public HomeController(ILogger<HomeController> logger,
                              UserManager<User> userManager,
                              IVendorService vendorService,
                              IDashboardService dashboardService)
        {
            _logger = logger;
            _userManager = userManager;
            _vendorService = vendorService;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return View(dashboardData);
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
    }
}
