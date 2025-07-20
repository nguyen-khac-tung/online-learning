using Microsoft.AspNetCore.Mvc;
using Online_Learning.Models.DTOs.Response.Admin;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Lấy thống kê tổng quan dashboard
        /// </summary>
        /// <param name="period">Khoảng thời gian: 7d, 30d, 90d, 1y</param>
        /// <returns>Thống kê tổng quan</returns>
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview([FromQuery] string period = "30d")
        {
            try
            {
                var result = await _dashboardService.GetOverviewAsync(period);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "OVERVIEW_ERROR",
                        message = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy thống kê doanh thu
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <param name="groupBy">Nhóm theo: day, week, month, year</param>
        /// <returns>Thống kê doanh thu</returns>
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string groupBy = "month")
        {
            try
            {
                startDate ??= DateTime.Now.AddDays(-30);
                endDate ??= DateTime.Now;

                var result = await _dashboardService.GetRevenueAsync(startDate.Value, endDate.Value, groupBy);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "REVENUE_ERROR",
                        message = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy dữ liệu cho biểu đồ doanh thu
        /// </summary>
        /// <param name="period">Khoảng thời gian: 7d, 30d, 90d</param>
        /// <param name="interval">Khoảng cách: day, week, month</param>
        /// <returns>Dữ liệu biểu đồ</returns>
        [HttpGet("revenue/chart")]
        public async Task<IActionResult> GetRevenueChart(
            [FromQuery] string period = "30d",
            [FromQuery] string interval = "day")
        {
            try
            {
                var result = await _dashboardService.GetRevenueChartAsync(period, interval);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "CHART_ERROR",
                        message = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy thống kê học viên
        /// </summary>
        /// <param name="period">Khoảng thời gian</param>
        /// <returns>Thống kê học viên</returns>
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents([FromQuery] string period = "30d")
        {
            try
            {
                var result = await _dashboardService.GetStudentsAsync(period);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "STUDENTS_ERROR",
                        message = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy phân tích học viên
        /// </summary>
        /// <returns>Phân tích học viên</returns>
        [HttpGet("students/analytics")]
        public async Task<IActionResult> GetStudentAnalytics()
        {
            try
            {
                var result = await _dashboardService.GetStudentAnalyticsAsync();
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "ANALYTICS_ERROR",
                        message = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy top khóa học
        /// </summary>
        /// <param name="limit">Số lượng khóa học</param>
        /// <param name="sortBy">Sắp xếp theo: revenue, enrollments, rating</param>
        /// <returns>Top khóa học</returns>
        [HttpGet("courses/top")]
        public async Task<IActionResult> GetTopCourses(
            [FromQuery] int limit = 10,
            [FromQuery] string sortBy = "revenue")
        {
            try
            {
                var result = await _dashboardService.GetTopCoursesAsync(limit, sortBy);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "TOP_COURSES_ERROR",
                        message = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy hiệu suất khóa học
        /// </summary>
        /// <param name="courseIds">Danh sách ID khóa học</param>
        /// <returns>Hiệu suất khóa học</returns>
        [HttpGet("courses/performance")]
        public async Task<IActionResult> GetCoursePerformance([FromQuery] string? courseIds = null)
        {
            try
            {
                var courseIdList = string.IsNullOrEmpty(courseIds)
                    ? new List<string>()
                    : courseIds.Split(',').ToList();

                var result = await _dashboardService.GetCoursePerformanceAsync(courseIdList);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "PERFORMANCE_ERROR",
                        message = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Export báo cáo
        /// </summary>
        /// <param name="type">Loại file: excel, pdf</param>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <param name="sections">Các phần cần export: revenue,students,courses</param>
        /// <returns>Link download báo cáo</returns>
        [HttpGet("reports/export")]
        public async Task<IActionResult> ExportReport(
    [FromQuery] string type = "excel",
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null,
    [FromQuery] string sections = "revenue,students,courses")
        {
            try
            {
                startDate ??= DateTime.Now.AddDays(-30);
                endDate ??= DateTime.Now;

                var sectionList = sections.Split(',').ToList();

                var excelBytes = await _dashboardService.ExportReportToExcelAsync(startDate.Value, endDate.Value, sectionList);
                var fileName = $"dashboard_report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(
                    fileContents: excelBytes,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: fileName
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "EXPORT_ERROR",
                        message = ex.Message
                    }
                });
            }
        }

    }
}