using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;

namespace Online_Learning.Repositories.Implementations
{
	public class CourseRepository : ICourseRepository
	{
		private readonly OnlineLearningContext _context;
		public CourseRepository(OnlineLearningContext context)
		{
			_context = context;
		}
		public IEnumerable<Models.DTOs.Response.User.CourseResponseDto> GetAllCourse()
		{
			return _context.Courses
				.Include(c => c.Language)
				.Include(c => c.Level)
				.Include(c => c.CourseCategories)
				.Select(x => new CourseResponseDto(x))
				.ToList();
		}

		public Models.DTOs.Response.User.CourseResponseDto GetCourseById(string id)
		{
			return _context.Courses
				.Include(c => c.Language)
				.Include(c => c.Level)
				.Include(c => c.CourseCategories)
				.Where(x => x.CourseId.Equals(id))
				.Select(x => new CourseResponseDto(x))
				.FirstOrDefault();
		}
	}
}
