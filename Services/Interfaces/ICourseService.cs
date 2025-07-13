using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Models.DTOs.Request.User;

namespace Online_Learning.Services.Interfaces
{
	public interface ICourseService
	{
		Task<IEnumerable<CourseResponseDTO>> GetAllCourseAsync();
		Task<PaginatedResponse<CourseResponseDTO>> GetCoursesWithFilterAsync(CourseRequestDto request);
		Task<CourseResponseDTO> GetCourseByIdAsync(string id);
		Task<CourseLearningResponseDTO> GetCourseLearningAsync(string courseId, string userId);
		Task<IEnumerable<CourseProgressResponseDTO>> GetCourseProgressByUserIdAsync(string userId, string? progress);
		void UpdateLessonProgress(string userId, long lessonId);
        Task<bool> CheckEnrollmentAsync(string userId, string courseId);
        Task<bool> UpdateQuizProgressAsync(string userId, long quizId);
    }
}
