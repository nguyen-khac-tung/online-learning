using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Models.DTOs.Request.User;

namespace Online_Learning.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        // USER - start
        // haipdhe
        Task<IEnumerable<Models.DTOs.Response.User.CourseResponseDTO>> GetAllCourseAsync();
        Task<PaginatedResponse<CourseResponseDTO>> GetCoursesWithFilterAsync(CourseRequestDto request);
        Task<Models.DTOs.Response.User.CourseResponseDTO> GetCourseByIdAsync(string id);
        Task<IEnumerable<Models.DTOs.Response.User.CourseProgressResponseDTO>> GetCourseByUserIdAsync(string userId, string? progress);
        Task<CourseLearningResponseDTO> GetCourseLearningAsync(string courseId, string userId);
        void UpdateLessonProgress(string userId, long lessonId);
        Task<bool> CheckEnrollmentAsync(string userId, string courseId);
        Task<bool> UpdateQuizProgressAsync(string userId, long quizId);
        // USER - end
    }
}
