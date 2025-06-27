using Online_Learning.Models.DTOs.Response.User;

namespace Online_Learning.Services.Interfaces
{
	public interface ICourseService
	{
		Task<IEnumerable<CourseResponseDTO>> GetAllCourseAsync();
		Task<CourseResponseDTO> GetCourseByIdAsync(string id);
		Task<IEnumerable<CourseProgressResponseDTO>> GetCourseProgressByUserIdAsync(string userId);
		void UpdateLessonProgress(string userId, long lessonId);
	}
}
