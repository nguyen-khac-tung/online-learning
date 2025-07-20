using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Online_Learning.Constants.Enums;
using Online_Learning.Models.DTOs.Response.Admin;
using Online_Learning.Models.Entities;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly OnlineLearningContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardService(OnlineLearningContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DashboardOverviewResponse> GetOverviewAsync(string period)
        {
            var (startDate, previousStartDate) = GetPeriodDates(period);
            var endDate = DateTime.Now;

            // Tính doanh thu hiện tại và trước đó
            var currentRevenue = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .SumAsync(o => o.TotalAmount);

            var previousRevenue = await _context.Orders
                .Where(o => o.OrderDate >= previousStartDate && o.OrderDate < startDate)
                .SumAsync(o => o.TotalAmount);

            // Tính số học viên
            var currentStudents = await _context.CourseEnrollments
                .Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate)
                .CountAsync();

            var previousStudents = await _context.CourseEnrollments
                .Where(e => e.CreatedAt >= previousStartDate && e.CreatedAt < startDate)
                .CountAsync();

            var activeStudents = await _context.Users
                .Where(u => u.Status == 1)
                .CountAsync();

            // Tính số khóa học
            var activeCourses = await _context.Courses
                .Where(c => c.Status == 1 && c.DeletedAt == null)
                .CountAsync();

            var publishedCourses = await _context.Courses
                .Where(c => c.PublishedAt != null && c.DeletedAt == null)
                .CountAsync();

            var draftCourses = await _context.Courses
                .Where(c => c.Status == 0 && c.DeletedAt == null)
                .CountAsync();

            return new DashboardOverviewResponse
            {
                TotalRevenue = new MetricData
                {
                    Current = (decimal)currentRevenue,
                    Previous = (decimal)previousRevenue,
                    GrowthRate = previousRevenue > 0
        ? ((decimal)(currentRevenue - previousRevenue) / (decimal)previousRevenue) * 100m
        : 0m,
                    Unit = "VND"
                },
                TotalStudents = new MetricData
                {
                    Current = currentStudents,
                    Previous = previousStudents,
                    GrowthRate = previousStudents > 0 ? ((decimal)(currentStudents - previousStudents) / previousStudents) * 100 : 0,
                    Active = activeStudents
                },
                TotalCourses = new CourseMetrics
                {
                    Active = activeCourses,
                    Published = publishedCourses,
                    Draft = draftCourses
                },
                ConversionRate = new MetricData
                {
                    Current = 12.5m, // Tính toán conversion rate thực tế
                    Previous = 10.2m,
                    GrowthRate = 22.5m,
                    Unit = "%"
                }
            };
        }

        public async Task<DashboardRevenueResponse> GetRevenueAsync(DateTime startDate, DateTime endDate, string groupBy)
        {
            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Course)
                .ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // Group revenue by time
            var revenueByTime = GroupRevenueByTime(orders, groupBy);

            // Revenue by course
            var revenueByCourse = await GetRevenueByCourse(startDate, endDate);

            // Revenue by payment method
            var revenueByPaymentMethod = orders
                .GroupBy(o => o.PaymetMethod)
                .Select(g => new RevenueByPaymentMethod
                {
                    Method = g.Key,
                    MethodName = GetPaymentMethodName(g.Key),
                    Revenue = (decimal)g.Sum(o => o.TotalAmount),
                    Percentage = totalRevenue > 0 ? ((decimal)g.Sum(o => o.TotalAmount) / (decimal)totalRevenue) * 100 : 0
                })
                .ToList();

            return new DashboardRevenueResponse
            {
                Summary = new RevenueSummary
                {
                    TotalRevenue = (decimal)totalRevenue,
                    TotalOrders = totalOrders,
                    AverageOrderValue = (decimal)averageOrderValue,
                    Currency = "VND"
                },
                RevenueByTime = revenueByTime,
                RevenueByCourse = revenueByCourse,
                RevenueByPaymentMethod = revenueByPaymentMethod
            };
        }

        public async Task<DashboardChartResponse> GetRevenueChartAsync(string period, string interval)
        {
            var (startDate, _) = GetPeriodDates(period);
            var endDate = DateTime.Now;

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();

            var groupedData = GroupDataByInterval(orders, interval, startDate, endDate);

            return new DashboardChartResponse
            {
                Labels = groupedData.Select(d => d.Label).ToList(),
                Datasets = new List<ChartDataset>
                {
                    new ChartDataset
                    {
                        Label = "Doanh thu",
                        Data = groupedData.Select(d => d.Revenue).ToList(),
                        BackgroundColor = "#3B82F6"
                    },
                    new ChartDataset
                    {
                        Label = "Số đơn hàng",
                        Data = groupedData.Select(d => d.Orders).ToList(),
                        BackgroundColor = "#10B981"
                    }
                }
            };
        }

        public async Task<DashboardStudentsResponse> GetStudentsAsync(string period)
        {
            var (startDate, _) = GetPeriodDates(period);
            var endDate = DateTime.Now;

            var totalStudents = await _context.Users.CountAsync();
            var newStudents = await _context.Users
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .CountAsync();

            var activeStudents = await _context.CourseEnrollments
                .Where(e => e.Status == 1)
                .Select(e => e.UserId)
                .Distinct()
                .CountAsync();

            var completedEnrollments = await _context.CourseEnrollments
                .Where(e => e.Progress == 100)
                .CountAsync();

            var totalEnrollments = await _context.CourseEnrollments.CountAsync();
            var completionRate = totalEnrollments > 0 ? ((decimal)completedEnrollments / totalEnrollments) * 100 : 0;

            // Students by time
            var newStudentsByTime = await GetNewStudentsByTime(startDate, endDate);

            // Students by status
            var studentsByStatus = await GetStudentsByStatus();

            return new DashboardStudentsResponse
            {
                Summary = new StudentSummary
                {
                    TotalStudents = totalStudents,
                    NewStudents = newStudents,
                    ActiveStudents = activeStudents,
                    CompletionRate = completionRate
                },
                NewStudentsByTime = newStudentsByTime,
                StudentsByStatus = studentsByStatus
            };
        }

        public async Task<DashboardStudentAnalyticsResponse> GetStudentAnalyticsAsync()
        {
            var users = await _context.Users.ToListAsync();

            // Age groups
            var ageGroups = users
                .Where(u => u.DoB.HasValue)
                .GroupBy(u => GetAgeGroup(u.DoB.Value))
                .Select(g => new AgeGroup
                {
                    Range = g.Key,
                    Count = g.Count(),
                    Percentage = ((decimal)g.Count() / users.Count) * 100
                })
                .ToList();

            // Gender distribution
            var genderGroups = users
                .GroupBy(u => u.Gender ?? false)
                .Select(g => new GenderGroup
                {
                    Type = g.Key ? "male" : "female",
                    Count = g.Count(),
                    Percentage = ((decimal)g.Count() / users.Count) * 100
                })
                .ToList();

            // Location distribution
            var locationGroups = users
                .Where(u => !string.IsNullOrEmpty(u.Address))
                .GroupBy(u => ExtractCity(u.Address))
                .Select(g => new LocationGroup
                {
                    City = g.Key,
                    Count = g.Count(),
                    Percentage = ((decimal)g.Count() / users.Count) * 100
                })
                .OrderByDescending(l => l.Count)
                .Take(10)
                .ToList();

            return new DashboardStudentAnalyticsResponse
            {
                Demographics = new StudentDemographics
                {
                    AgeGroups = ageGroups,
                    Gender = genderGroups,
                    Locations = locationGroups
                },
                Retention = new StudentRetention
                {
                    Day1 = 95.2m,
                    Day7 = 78.5m,
                    Day30 = 65.3m,
                    Day90 = 48.7m
                }
            };
        }

        public async Task<DashboardTopCoursesResponse> GetTopCoursesAsync(int limit, string sortBy)
        {
            var coursesQuery = _context.Courses
                .Include(c => c.CourseEnrollments)
                .Include(c => c.OrderItems)
                .Include(c => c.CourseCategories)
                .ThenInclude(cc => cc.Category)
                .Where(c => c.DeletedAt == null);

            var courses = await coursesQuery.ToListAsync();

            var topByRevenue = courses
                .Select(c => new TopCourse
                {
                    CourseId = c.CourseId,
                    Title = c.CourseName,
                    Instructor = c.Creator,
                    Revenue = (decimal)c.OrderItems.Sum(oi => oi.Price),
                    Enrollments = c.CourseEnrollments.Count,
                    Rating = 4.5m, // Tính từ bảng rating thực tế
                    Category = c.CourseCategories.FirstOrDefault()?.Category?.CategoryName ?? "",
                    CompletionRate = c.CourseEnrollments.Count > 0 
                        ? ((decimal)c.CourseEnrollments.Count(e => e.Progress == 100) / c.CourseEnrollments.Count) * 100 
                        : 0
                })
                .OrderByDescending(c => c.Revenue)
                .Take(limit)
                .ToList();

            var topByEnrollments = courses
                .Select(c => new TopCourse
                {
                    CourseId = c.CourseId,
                    Title = c.CourseName,
                    Instructor = c.Creator,
                    Revenue = (decimal)c.OrderItems.Sum(oi => oi.Price),
                    Enrollments = c.CourseEnrollments.Count,
                    Rating = 4.5m,
                    Category = c.CourseCategories.FirstOrDefault()?.Category?.CategoryName ?? "",
                    CompletionRate = c.CourseEnrollments.Count > 0 
                        ? ((decimal)c.CourseEnrollments.Count(e => e.Progress == 100) / c.CourseEnrollments.Count) * 100 
                        : 0
                })
                .OrderByDescending(c => c.Enrollments)
                .Take(limit)
                .ToList();

            // Trending courses (courses with high growth in last 7 days)
            var weekAgo = DateTime.Now.AddDays(-7);
            var trending = courses
                .Where(c => c.CourseEnrollments.Any(e => e.CreatedAt >= weekAgo))
                .Select(c => new TrendingCourse
                {
                    CourseId = c.CourseId,
                    Title = c.CourseName,
                    Enrollments = c.CourseEnrollments.Count(e => e.CreatedAt >= weekAgo),
                    GrowthRate = CalculateGrowthRate(c.CourseEnrollments.ToList(), weekAgo),
                    Period = "7 ngày qua"
                })
                .OrderByDescending(c => c.GrowthRate)
                .Take(limit)
                .ToList();

            return new DashboardTopCoursesResponse
            {
                TopByRevenue = topByRevenue,
                TopByEnrollments = topByEnrollments,
                Trending = trending
            };
        }

        public async Task<List<DashboardCoursePerformanceResponse>> GetCoursePerformanceAsync(List<string> courseIds)
        {
            var query = _context.Courses
                .Include(c => c.CourseEnrollments)
                .Include(c => c.OrderItems)
                .Where(c => c.DeletedAt == null);

            if (courseIds.Any())
            {
                query = query.Where(c => courseIds.Contains(c.CourseId));
            }

            var courses = await query.ToListAsync();

            return courses.Select(c => new DashboardCoursePerformanceResponse
            {
                CourseId = c.CourseId,
                Title = c.CourseName,
                Metrics = new CourseMetrics
                {
                    // Note: Using CourseMetrics from the existing class, adjust as needed
                    Active = c.CourseEnrollments.Count,
                    Published = c.Status == 1 ? 1 : 0,
                    Draft = c.Status == 0 ? 1 : 0
                },
                ProgressDistribution = GetProgressDistribution(c.CourseEnrollments.ToList())
            }).ToList();
        }

        public async Task<byte[]> ExportReportToExcelAsync(DateTime startDate, DateTime endDate, List<string> sections)
{
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    using var package = new ExcelPackage();

    // 1. Revenue
    if (sections.Contains("revenue"))
    {
        var revenueData = await GetRevenueAsync(startDate, endDate, "day");
        var revenueByTimeList = revenueData.RevenueByTime;

        var sheet = package.Workbook.Worksheets.Add("Doanh thu");
        sheet.Cells[1, 1].Value = "Ngày";
        sheet.Cells[1, 2].Value = "Tổng doanh thu";

        for (int i = 0; i < revenueByTimeList.Count; i++)
        {
            var row = i + 2;
            sheet.Cells[row, 1].Value = revenueByTimeList[i].Period;
            sheet.Cells[row, 2].Value = revenueByTimeList[i].Revenue;
        }
    }

    // 2. Students
    if (sections.Contains("students"))
    {
        var studentsData = await GetStudentsAsync("30d");
        var summary = studentsData.Summary;

        var sheet = package.Workbook.Worksheets.Add("Học viên");
        sheet.Cells[1, 1].Value = "Thông tin";
        sheet.Cells[1, 2].Value = "Giá trị";

        sheet.Cells[2, 1].Value = "Tổng học viên";
        sheet.Cells[2, 2].Value = summary.TotalStudents;

        sheet.Cells[3, 1].Value = "Học viên mới";
        sheet.Cells[3, 2].Value = summary.NewStudents;

        sheet.Cells[4, 1].Value = "Học viên đang hoạt động";
        sheet.Cells[4, 2].Value = summary.ActiveStudents;

        sheet.Cells[5, 1].Value = "Tỷ lệ hoàn thành";
        sheet.Cells[5, 2].Value = $"{summary.CompletionRate:P2}";
    }

    // 3. Courses
    if (sections.Contains("courses"))
    {
        var topCourses = await GetTopCoursesAsync(10, "revenue");
        var list = topCourses.TopByRevenue;

        var sheet = package.Workbook.Worksheets.Add("Khóa học");
        sheet.Cells[1, 1].Value = "Tên khóa học";
        sheet.Cells[1, 2].Value = "Doanh thu";
        sheet.Cells[1, 3].Value = "Số lượt đăng ký";

        for (int i = 0; i < list.Count; i++)
        {
            var row = i + 2;
            sheet.Cells[row, 1].Value = list[i].Title;
            sheet.Cells[row, 2].Value = list[i].Revenue;
            sheet.Cells[row, 3].Value = list[i].Enrollments;
        }
    }

    return package.GetAsByteArray();
}




        #region Private Helper Methods

        private (DateTime startDate, DateTime previousStartDate) GetPeriodDates(string period)
        {
            var now = DateTime.Now;
            return period switch
            {
                "7d" => (now.AddDays(-7), now.AddDays(-14)),
                "30d" => (now.AddDays(-30), now.AddDays(-60)),
                "90d" => (now.AddDays(-90), now.AddDays(-180)),
                "1y" => (now.AddYears(-1), now.AddYears(-2)),
                _ => (now.AddDays(-30), now.AddDays(-60))
            };
        }

        private List<RevenueByTime> GroupRevenueByTime(List<Order> orders, string groupBy)
        {
            return groupBy switch
            {
                "day" => orders
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new RevenueByTime
                    {
                        Period = g.Key.ToString("yyyy-MM-dd"),
                        Revenue = (decimal)g.Sum(o => o.TotalAmount),
                        Orders = g.Count(),
                        NewCustomers = g.Select(o => o.UserId).Distinct().Count()
                    })
                    .OrderBy(r => r.Period)
                    .ToList(),
                
                "week" => orders
                    .GroupBy(o => GetWeekStart(o.OrderDate))
                    .Select(g => new RevenueByTime
                    {
                        Period = g.Key.ToString("yyyy-MM-dd"),
                        Revenue = (decimal)g.Sum(o => o.TotalAmount),
                        Orders = g.Count(),
                        NewCustomers = g.Select(o => o.UserId).Distinct().Count()
                    })
                    .OrderBy(r => r.Period)
                    .ToList(),
                
                "month" => orders
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .Select(g => new RevenueByTime
                    {
                        Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Revenue = (decimal)g.Sum(o => o.TotalAmount),
                        Orders = g.Count(),
                        NewCustomers = g.Select(o => o.UserId).Distinct().Count()
                    })
                    .OrderBy(r => r.Period)
                    .ToList(),
                
                _ => orders
                    .GroupBy(o => o.OrderDate.Year)
                    .Select(g => new RevenueByTime
                    {
                        Period = g.Key.ToString(),
                        Revenue = (decimal)g.Sum(o => o.TotalAmount),
                        Orders = g.Count(),
                        NewCustomers = g.Select(o => o.UserId).Distinct().Count()
                    })
                    .OrderBy(r => r.Period)
                    .ToList()
            };
        }

        private async Task<List<RevenueByCourse>> GetRevenueByCourse(DateTime startDate, DateTime endDate)
        {
            return await _context.OrderItems
                .Include(oi => oi.Course)
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .GroupBy(oi => new { oi.CourseId, oi.Course.CourseName })
                .Select(g => new RevenueByCourse
                {
                    CourseId = g.Key.CourseId,
                    CourseName = g.Key.CourseName,
                    Revenue = (decimal)g.Sum(oi => oi.Price),
                    Enrollments = g.Count(),
                    Price = (decimal)g.Average(oi => oi.Price)
                })
                .OrderByDescending(r => r.Revenue)
                .ToListAsync();
        }

        private string GetPaymentMethodName(string method)
        {
            return method switch
            {
                "bank_transfer" => "Chuyển khoản",
                "momo" => "MoMo",
                "zalopay" => "ZaloPay",
                "vnpay" => "VNPay",
                "credit_card" => "Thẻ tín dụng",
                _ => method
            };
        }

        private List<ChartData> GroupDataByInterval(List<Order> orders, string interval, DateTime startDate, DateTime endDate)
        {
            var result = new List<ChartData>();
            var current = startDate;

            while (current <= endDate)
            {
                var next = interval switch
                {
                    "day" => current.AddDays(1),
                    "week" => current.AddDays(7),
                    "month" => current.AddMonths(1),
                    _ => current.AddDays(1)
                };

                var periodOrders = orders.Where(o => o.OrderDate >= current && o.OrderDate < next).ToList();
                
                result.Add(new ChartData
                {
                    Label = current.ToString("yyyy-MM-dd"),
                    Revenue = (decimal)periodOrders.Sum(o => o.TotalAmount),
                    Orders = periodOrders.Count
                });

                current = next;
            }

            return result;
        }

        private async Task<List<StudentsByTime>> GetNewStudentsByTime(DateTime startDate, DateTime endDate)
        {
            var groupedStudents = await _context.Users
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .GroupBy(u => u.CreatedAt.Date)
                .ToListAsync();

            var result = groupedStudents
                .Select(g => new StudentsByTime
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count(),
                    Cumulative = 0
                })
                .OrderBy(s => s.Date)
                .ToList();

            return result;
        }

        private async Task<List<StudentsByStatus>> GetStudentsByStatus()
        {
            var totalUsers = await _context.Users.CountAsync();
            
            return await _context.Users
                .GroupBy(u => u.Status)
                .Select(g => new StudentsByStatus
                {
                    Status = g.Key == 1 ? "active" : "inactive",
                    StatusName = g.Key == 1 ? "Đang hoạt động" : "Không hoạt động",
                    Count = g.Count(),
                    Percentage = totalUsers > 0 ? ((decimal)g.Count() / totalUsers) * 100 : 0
                })
                .ToListAsync();
        }

        private string GetAgeGroup(DateOnly birthDate)
        {
            var age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear) age--;

            return age switch
            {
                < 18 => "Dưới 18",
                >= 18 and <= 25 => "18-25",
                >= 26 and <= 35 => "26-35",
                >= 36 and <= 45 => "36-45",
                >= 46 and <= 55 => "46-55",
                _ => "Trên 55"
            };
        }

        private string ExtractCity(string address)
        {
            // Simple city extraction - you may want to improve this
            var parts = address.Split(',');
            return parts.LastOrDefault()?.Trim() ?? "Khác";
        }

        private DateTime GetWeekStart(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        private decimal CalculateGrowthRate(List<CourseEnrollment> enrollments, DateTime compareDate)
        {
            var currentCount = enrollments.Count(e => e.CreatedAt >= compareDate);
            var previousCount = enrollments.Count(e => e.CreatedAt < compareDate);
            
            return previousCount > 0 ? ((decimal)(currentCount - previousCount) / previousCount) * 100 : 0;
        }

        private List<ProgressDistribution> GetProgressDistribution(List<CourseEnrollment> enrollments)
        {
            var total = (decimal)enrollments.Count;
            if (total == 0) return new List<ProgressDistribution>();

            return new List<ProgressDistribution>
            {
                new() {
                    Range = "0-25%",
                    Count = enrollments.Count(e => e.Progress >= 0 && e.Progress < 25),
                    Percentage = (enrollments.Count(e => e.Progress >= 0 && e.Progress < 25) / total) * 100
                },
                new() {
                    Range = "26-50%",
                    Count = enrollments.Count(e => e.Progress >= 25 && e.Progress < 50),
                    Percentage = (enrollments.Count(e => e.Progress >= 25 && e.Progress < 50) / total) * 100
                },
                new() {
                    Range = "51-75%",
                    Count = enrollments.Count(e => e.Progress >= 50 && e.Progress < 75),
                    Percentage = (enrollments.Count(e => e.Progress >= 50 && e.Progress < 75) / total) * 100
                },
                new() {
                    Range = "76-100%",
                    Count = enrollments.Count(e => e.Progress >= 75),
                    Percentage = (enrollments.Count(e => e.Progress >= 75) / total) * 100
                }
            };
        }

        #endregion

        #region Helper Classes

        private class ChartData
        {
            public string Label { get; set; } = "";
            public decimal Revenue { get; set; }
            public decimal Orders { get; set; }
        }

        #endregion
    }
}