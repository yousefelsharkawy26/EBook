using Digital_Library.Core.ViewModels;

namespace Digital_Library.Service.Interface;

// en Core/Interfaces/IDashboardService.cs
public interface IDashboardService
{
    // Para obtener todos los datos de la página principal del dashboard
    Task<DashboardViewModel> GetDashboardDataAsync();

    // Para obtener los datos de los gráficos
    Task<ChartDataViewModel> GetUserGrowthChartDataAsync(int days = 30);
    Task<ChartDataViewModel> GetSalesChartDataAsync(int days = 30);
}
