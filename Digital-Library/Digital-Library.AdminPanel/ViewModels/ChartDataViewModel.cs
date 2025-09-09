namespace Digital_Library.AdminPanel.ViewModels;
public class ChartDataViewModel
{
    // قائمة بالتسميات (Labels)، ستكون التواريخ
    public List<string> Labels { get; set; } = new List<string>();

    // قائمة بالبيانات (Data)، ستكون العدد التراكمي للمستخدمين
    public List<int> Data { get; set; } = new List<int>();
}
