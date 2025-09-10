using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Digital_Library.AdminPanel.Controllers;

// en Web/Controllers/DashboardApiController.cs
[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("GetUserGrowthChartData")]
    public async Task<IActionResult> GetUserGrowthChartData()
    {
        var chartData = await _dashboardService.GetUserGrowthChartDataAsync();
        return Ok(chartData);
    }

    [HttpGet("GetSalesChartData")]
    public async Task<IActionResult> GetSalesChartData()
    {
        var chartData = await _dashboardService.GetSalesChartDataAsync();
        return Ok(chartData);
    }
}
