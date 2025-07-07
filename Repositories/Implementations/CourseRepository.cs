using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Models.DTOs.Request.User;
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
				.Include(c => c.CourseEnrollments)
				.Include(c => c.Modules).ThenInclude(c => c.Lessons)
				.Where(c => c.Status == (int)CourseStatus.Published)
				.Select(x => new CourseResponseDTO(x))
				.ToListAsync();

			return obj;
		}

		// USER - GET COURSE LIST WITH FILTER, SEARCH, SORT, PAGINATION (haipdhe172178)
		public async Task<PaginatedResponse<CourseResponseDTO>> GetCoursesWithFilterAsync(CourseRequestDto request)
		{
			// Validate và set default values
			request.Page = Math.Max(1, request.Page);
			request.PageSize = Math.Max(1, Math.Min(6, request.PageSize));

			var query = _context.Courses
				.Include(c => c.Language)
				.Include(c => c.Level)
				.Include(c => c.CoursePrices)
				.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category)
				.Include(c => c.CourseImages)
				.Include(c => c.CourseEnrollments)
				.Include(c => c.Modules).ThenInclude(c => c.Lessons)
				.Where(c => c.Status == (int)CourseStatus.Published);

			// Search course by cName
			if (!string.IsNullOrWhiteSpace(request.SearchTerm))
			{
				var searchTerm = request.SearchTerm.ToLower();
				query = query.Where(c => c.CourseName.ToLower().Contains(searchTerm));
			}

			// Filter theo Level
			//if (request.LevelId.HasValue)
			//{
			//	query = query.Where(c => c.LevelId == request.LevelId.Value);
			//}

			// Filter theo Language
			//if (request.LanguageId.HasValue)
			//{
			//	query = query.Where(c => c.LanguageId == request.LanguageId.Value);
			//}

			// Filter theo Category
			if (request.CategoryIds != null && request.CategoryIds.Any())
			{
				query = query.Where(c => c.CourseCategories.Any(cc => request.CategoryIds.Contains(cc.CategoryId)));
			}

			// Filter theo khoảng giá
			if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
			{
				query = query.Where(c => c.CoursePrices.Any(cp =>
					(!request.MinPrice.HasValue || cp.Price >= request.MinPrice.Value) &&
					(!request.MaxPrice.HasValue || cp.Price <= request.MaxPrice.Value)));
			}

			// Sắp xếp
			query = ApplySorting(query, request.SortBy, request.SortOrder);

			// Đếm tổng số records trước khi phân trang
			var totalCount = await query.CountAsync();

			// Phân trang
			var courses = await query
				.Skip((request.Page - 1) * request.PageSize)
				.Take(request.PageSize)
				.Select(x => new CourseResponseDTO(x))
				.ToListAsync();

			// Tính toán thông tin phân trang
			var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

			return new PaginatedResponse<CourseResponseDTO>
			{
				DataPaginated = courses,
				CurrentPage = request.Page,
				PageSize = request.PageSize,
				TotalCount = totalCount,
				TotalPages = totalPages,
				HasNextPage = request.Page < totalPages,
				HasPreviousPage = request.Page > 1
			};
		}

		private IQueryable<Course> ApplySorting(IQueryable<Course> query, string? sortBy, string? sortOrder)
		{
			var isDescending = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToLower() == "desc";

			switch (sortBy?.ToLower())
			{
				case "name":
					return isDescending ? query.OrderByDescending(c => c.CourseName) : query.OrderBy(c => c.CourseName);

				case "price":
					return isDescending
						? query.OrderByDescending(c => c.CoursePrices.OrderByDescending(cp => cp.CreateAt).Select(cp => cp.Price).FirstOrDefault())
						: query.OrderBy(c => c.CoursePrices.OrderByDescending(cp => cp.CreateAt).Select(cp => cp.Price).FirstOrDefault());

				case "createdat":
					return isDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt);

				case "enrollmentcount":
					return isDescending
						? query.OrderByDescending(c => c.CourseEnrollments.Count)
						: query.OrderBy(c => c.CourseEnrollments.Count);

				case "lessoncount":
					return isDescending
						? query.OrderByDescending(c => c.Modules.SelectMany(m => m.Lessons).Count())
						: query.OrderBy(c => c.Modules.SelectMany(m => m.Lessons).Count());

				default:
					// Mặc định sắp xếp theo thời gian tạo mới nhất
					return query.OrderByDescending(c => c.CreatedAt);
			}
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
		public async Task<IEnumerable<CourseProgressResponseDTO>> GetCourseByUserIdAsync(string userId, string? progress)
		{
			if (string.IsNullOrEmpty(userId))
			{
				return Enumerable.Empty<CourseProgressResponseDTO>();
			}

			// Bắt đầu query
			var query = _context.CourseEnrollments
								.Include(c => c.Course)
								.Where(c => c.UserId == userId);

			// Gắn điều kiện theo status
			if (!string.IsNullOrEmpty(progress))
			{
				switch (progress.ToLower())
				{
					case "in-progress":
						query = query.Where(c => c.Progress < 100);
						break;
					case "completed":
						query = query.Where(c => c.Progress >= 100);
						break;
				}
			}
			else
			{
				//default 
				query = query.Where(c => c.Progress < 100);
			}

			// Dựng kết quả
			var result = await query
				.Select(c => new CourseProgressResponseDTO(c.Course, c.Progress))
				.ToListAsync();

			return result;
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
