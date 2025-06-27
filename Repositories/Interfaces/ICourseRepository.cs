
using Online_Learning.Models.DTOs.Response.User;

namespace Online_Learning.Repositories.Interfaces
{
	public interface ICourseRepository
	{
		// USER - start
		// haipdhe
		Task<IEnumerable<Models.DTOs.Response.User.CourseResponseDTO>> GetAllCourseAsync();
		Task<Models.DTOs.Response.User.CourseResponseDTO> GetCourseByIdAsync(string id);
		Task<IEnumerable<Models.DTOs.Response.User.CourseProgressResponseDTO>> GetCourseByUserIdAsync(string userId);
		void UpdateLessonProgress(string userId, long lessonId);
		// USER - end
	}
}
