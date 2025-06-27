using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Constants.Enums;

namespace Online_Learning.Repositories.Implementations
{
	public class CourseRepository : ICourseRepository
	{
		private readonly OnlineLearningContext _context;
		public CourseRepository(OnlineLearningContext context)
		{
			_context = context;
		}
		// USER - GET COURSE LIST (haipdhe172178)
		public async Task<IEnumerable<Models.DTOs.Response.User.CourseResponseDTO>> GetAllCourseAsync()
		{
			var obj = await _context.Courses
				.Include(c => c.Language)
				.Include(c => c.Level)
				.Include(c => c.CoursePrices)
				.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category)
				.Include(c => c.CourseImages)
				.Where(c => c.Status == (int)CourseStatus.Published)
				.Select(x => new CourseResponseDTO(x))
				.ToListAsync();

			return obj;
		}

		// USER - GET COURSE DETAIL BY COURSE ID (haipdhe172178)
		public async Task<CourseResponseDTO> GetCourseByIdAsync(string courseId)
		{
			if (string.IsNullOrEmpty(courseId))
			{
				return null;
			}

			var obj = await _context.Courses
				.Include(c => c.Language)
				.Include(c => c.Level)
				.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category)
				.Include(c => c.Modules)
					.ThenInclude(m => m.Lessons)
				.Include(c => c.Modules)
					.ThenInclude(m => m.Quizzes)
				.Include(c => c.CoursePrices)
				.Include(c => c.CourseImages)
				.Where(x => x.CourseId == courseId && x.Status == (int)CourseStatus.Published)
				.Select(x => new CourseResponseDTO(x))
				.FirstOrDefaultAsync();

			return obj;
		}

		// MY LEARNING - GET COURSE BY USER ID (haipdhe172178)
		public async Task<IEnumerable<CourseProgressResponseDTO>> GetCourseByUserIdAsync(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				return Enumerable.Empty<CourseProgressResponseDTO>();
			}
			var courseEnrollment = await _context.CourseEnrollments
										.Include(c => c.Course)
										.Where(c => c.UserId.Equals(userId))
										.Select(c => new CourseProgressResponseDTO(c.Course, c.Progress))
										.ToListAsync();
			return courseEnrollment;
		}

		public void UpdateLessonProgress(string userId, long lessonId)
		{
			var obj = _context.LessonProgresses
						.FirstOrDefault(c => c.UserId.Equals(userId) && c.LessonId.Equals(lessonId));
			if (obj != null) { return; }
			_context.LessonProgresses.Add(new LessonProgress
			{
				UserId = userId,
				LessonId = lessonId,
				CompletedAt = DateTime.UtcNow,
				IsCompleted = true
			});
			_context.SaveChanges();
		}
	}
}
