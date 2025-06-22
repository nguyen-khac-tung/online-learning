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
        public IEnumerable<CourseResponseDto> GetAllCourse()
		{
			return _courseRepository.GetAllCourse();
		}

		public CourseResponseDto GetCourseById(string id)
		{
			return _courseRepository.GetCourseById(id);
		}
	}
}
