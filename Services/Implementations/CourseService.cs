using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Services.Implementations
{
	public class CourseService : ICourseService
	{
		private readonly ICourseRepository _courseRepository;
		public CourseService(ICourseRepository courseRepository)
		{
			_courseRepository = courseRepository;
		}
		public async Task<IEnumerable<CourseResponseDTO>> GetAllCourseAsync()
		{
			return await _courseRepository.GetAllCourseAsync();
		}

		public async Task<CourseResponseDTO> GetCourseByIdAsync(string id)
		{
			return await _courseRepository.GetCourseByIdAsync(id);
		}

		public async Task<IEnumerable<CourseProgressResponseDTO>> GetCourseProgressByUserIdAsync(string userId)
		{
			return await _courseRepository.GetCourseByUserIdAsync(userId);
		}

		public void UpdateLessonProgress(string userId, long lessonId)
		{
			_courseRepository.UpdateLessonProgress(userId, lessonId);
		}
	}
}
