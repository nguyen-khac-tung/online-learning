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
		Task<IEnumerable<CourseProgressResponseDTO>> GetCourseProgressByUserIdAsync(string userId);
		void UpdateLessonProgress(string userId, long lessonId);
	}
}
