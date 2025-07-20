using Online_Learning.Models.DTOs.Response.Admin;

namespace Online_Learning.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardOverviewResponse> GetOverviewAsync(string period);
        Task<DashboardRevenueResponse> GetRevenueAsync(DateTime startDate, DateTime endDate, string groupBy);
        Task<DashboardChartResponse> GetRevenueChartAsync(string period, string interval);
        Task<DashboardStudentsResponse> GetStudentsAsync(string period);
        Task<DashboardStudentAnalyticsResponse> GetStudentAnalyticsAsync();
        Task<DashboardTopCoursesResponse> GetTopCoursesAsync(int limit, string sortBy);
        Task<List<DashboardCoursePerformanceResponse>> GetCoursePerformanceAsync(List<string> courseIds);
        Task<byte[]> ExportReportToExcelAsync(DateTime startDate, DateTime endDate, List<string> sections);
    }
}
