using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.Course;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Online_Learning.Models.DTOs.Request.Admin.Course;
using Online_Learning.Services.Interfaces;
using Online_Learning.Services.Interfaces.Admin;

namespace Online_Learning.Repositories.Interfaces.Admin
{
    public interface ICourseRepository
    {
        Task<IEnumerable<CourseResponseDto>> GetCoursesAsync(int page, int pageSize, string? search, int? status);
        Task<CourseResponseDto?> GetCourseByIdAsync(string id);
        Task<int> GetTotalCountAsync(string? search, int? status);
        Task<(Course, List<string>, CoursePrice?)> CreateCourseAsync(Course course, List<int>? categoryIds, decimal? price, List<IFormFile>? attachmentFiles, IFileService fileService);
        Task<bool> UpdateCourseAsync(string id, CourseUpdateDto courseDto, IFileService fileService);
        Task<bool> DeleteCourseAsync(string id, IFileService fileService);
        // Có thể bổ sung các method đặc thù khác nếu cần
    }
} 