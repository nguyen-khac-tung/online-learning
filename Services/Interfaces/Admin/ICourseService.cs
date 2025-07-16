using Online_Learning.Models.DTOs.Request.Admin.Course;
using Online_Learning.Models.DTOs.Response.Admin.Course;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Services.Interfaces.Admin
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseResponseDto>> GetCoursesAsync(int page, int pageSize, string? search, int? status);
        Task<CourseResponseDto?> GetCourseByIdAsync(string id);
        Task<CourseResponseDto> CreateCourseAsync(CourseCreateDto courseDto);
        Task<bool> UpdateCourseAsync(string id, CourseUpdateDto courseDto);
        Task<bool> DeleteCourseAsync(string id);
        Task<int> GetTotalCountAsync(string? search, int? status);
    }
}
