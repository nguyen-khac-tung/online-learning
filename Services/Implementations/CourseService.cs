using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Models.DTOs.Request.User;
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

		public async Task<PaginatedResponse<CourseResponseDTO>> GetCoursesWithFilterAsync(CourseRequestDto request)
		{
			return await _courseRepository.GetCoursesWithFilterAsync(request);
		}

		public async Task<CourseResponseDTO> GetCourseByIdAsync(string id)
		{
			return await _courseRepository.GetCourseByIdAsync(id);
		}

		public async Task<IEnumerable<CourseProgressResponseDTO>> GetCourseProgressByUserIdAsync(string userId,string? progress)
		{
			return await _courseRepository.GetCourseByUserIdAsync(userId, progress);
		}

		public void UpdateLessonProgress(string userId, long lessonId)
		{
			_courseRepository.UpdateLessonProgress(userId, lessonId);
		}
	}
}
