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
		private readonly ILesssonRepository _lesssonRepository;
		public CourseService(ICourseRepository courseRepository, ILesssonRepository lesssonRepository)
		{
			_courseRepository = courseRepository;
			_lesssonRepository = lesssonRepository;
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

		public async Task<IEnumerable<CourseProgressResponseDTO>> GetCourseProgressByUserIdAsync(string userId, string? progress)
		{
			return await _courseRepository.GetCourseByUserIdAsync(userId, progress);
		}

		public void UpdateLessonProgress(string userId, long lessonId)
		{
			_courseRepository.UpdateLessonProgress(userId, lessonId);
		}

		public async Task<CourseLearningResponseDTO> GetCourseLearningAsync(string courseId, string userId)
		{
			return await _courseRepository.GetCourseLearningAsync(courseId, userId);
		}

        public async Task<bool> CheckEnrollmentAsync(string userId, string courseId)
        {
            return await _courseRepository.CheckEnrollmentAsync(userId, courseId);
        }

        public async Task<bool> UpdateQuizProgressAsync(string userId, long quizId)
        {
            try
            {
                return await _courseRepository.UpdateQuizProgressAsync(userId, quizId);
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetProgressAsync(string userId, string courseId)
        {
            return await _courseRepository.GetProgressAsync(userId, courseId);
        }

        public async Task<bool> EnrollCourseAsync(string userId, string courseId)
        {
            return await _courseRepository.EnrollCourseAsync(userId, courseId);
        }
    }
}
