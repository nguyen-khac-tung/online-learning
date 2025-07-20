namespace Online_Learning.Models.DTOs.Response.Admin
{
    public class DashboardOverviewResponse
    {
        public MetricData TotalRevenue { get; set; } = new();
        public MetricData TotalStudents { get; set; } = new();
        public CourseMetrics TotalCourses { get; set; } = new();
        public MetricData ConversionRate { get; set; } = new();
    }

    public class MetricData
    {
        public decimal Current { get; set; }
        public decimal Previous { get; set; }
        public decimal GrowthRate { get; set; }
        public string Unit { get; set; } = "VND";
        public int? Active { get; set; }
    }

    public class CourseMetrics
    {
        public int Active { get; set; }
        public int Published { get; set; }
        public int Draft { get; set; }
    }

    public class DashboardRevenueResponse
    {
        public RevenueSummary Summary { get; set; } = new();
        public List<RevenueByTime> RevenueByTime { get; set; } = new();
        public List<RevenueByCourse> RevenueByCourse { get; set; } = new();
        public List<RevenueByPaymentMethod> RevenueByPaymentMethod { get; set; } = new();
    }

    public class RevenueSummary
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string Currency { get; set; } = "VND";
    }

    public class RevenueByTime
    {
        public string Period { get; set; } = "";
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
        public int NewCustomers { get; set; }
    }

    public class RevenueByCourse
    {
        public string CourseId { get; set; } = "";
        public string CourseName { get; set; } = "";
        public decimal Revenue { get; set; }
        public int Enrollments { get; set; }
        public decimal Price { get; set; }
    }

    public class RevenueByPaymentMethod
    {
        public string Method { get; set; } = "";
        public string MethodName { get; set; } = "";
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
    }

    public class DashboardChartResponse
    {
        public List<string> Labels { get; set; } = new();
        public List<ChartDataset> Datasets { get; set; } = new();
    }

    public class ChartDataset
    {
        public string Label { get; set; } = "";
        public List<decimal> Data { get; set; } = new();
        public string BackgroundColor { get; set; } = "";
    }

    public class DashboardStudentsResponse
    {
        public StudentSummary Summary { get; set; } = new();
        public List<StudentsByTime> NewStudentsByTime { get; set; } = new();
        public List<StudentsByStatus> StudentsByStatus { get; set; } = new();
    }

    public class StudentSummary
    {
        public int TotalStudents { get; set; }
        public int NewStudents { get; set; }
        public int ActiveStudents { get; set; }
        public decimal CompletionRate { get; set; }
    }

    public class StudentsByTime
    {
        public string Date { get; set; } = "";
        public int Count { get; set; }
        public int Cumulative { get; set; }
    }

    public class StudentsByStatus
    {
        public string Status { get; set; } = "";
        public string StatusName { get; set; } = "";
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class DashboardStudentAnalyticsResponse
    {
        public StudentDemographics Demographics { get; set; } = new();
        public StudentRetention Retention { get; set; } = new();
    }

    public class StudentDemographics
    {
        public List<AgeGroup> AgeGroups { get; set; } = new();
        public List<GenderGroup> Gender { get; set; } = new();
        public List<LocationGroup> Locations { get; set; } = new();
    }

    public class AgeGroup
    {
        public string Range { get; set; } = "";
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class GenderGroup
    {
        public string Type { get; set; } = "";
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class LocationGroup
    {
        public string City { get; set; } = "";
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class StudentRetention
    {
        public decimal Day1 { get; set; }
        public decimal Day7 { get; set; }
        public decimal Day30 { get; set; }
        public decimal Day90 { get; set; }
    }

    public class DashboardTopCoursesResponse
    {
        public List<TopCourse> TopByRevenue { get; set; } = new();
        public List<TopCourse> TopByEnrollments { get; set; } = new();
        public List<TrendingCourse> Trending { get; set; } = new();
    }

    public class TopCourse
    {
        public string CourseId { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Thumbnail { get; set; }
        public string Instructor { get; set; } = "";
        public decimal Price { get; set; }
        public decimal Revenue { get; set; }
        public int Enrollments { get; set; }
        public decimal Rating { get; set; }
        public string Category { get; set; } = "";
        public decimal CompletionRate { get; set; }
    }

    public class TrendingCourse
    {
        public string CourseId { get; set; } = "";
        public string Title { get; set; } = "";
        public int Enrollments { get; set; }
        public decimal GrowthRate { get; set; }
        public string Period { get; set; } = "";
    }

    public class DashboardCoursePerformanceResponse
    {
        public string CourseId { get; set; } = "";
        public string Title { get; set; } = "";
        public CourseMetrics Metrics { get; set; } = new();
        public List<ProgressDistribution> ProgressDistribution { get; set; } = new();
    }

    public class CoursePerformanceMetrics
    {
        public int Enrollments { get; set; }
        public int Completions { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int AverageStudyTime { get; set; }
        public decimal Revenue { get; set; }
    }

    public class ProgressDistribution
    {
        public string Range { get; set; } = "";
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class DashboardExportResponse
    {
        public string DownloadUrl { get; set; } = "";
        public string FileName { get; set; } = "";
        public long FileSize { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
