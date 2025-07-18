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
        private readonly ILesssonRepository _lesssonRepository;
        public CourseRepository(OnlineLearningContext context, ILesssonRepository lesssonRepository)
        {
            _context = context;
            _lesssonRepository = lesssonRepository;
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

        // USER - GET COURSE INFO FOR LEARNING FEATURE (haipdhe172178)
        public async Task<CourseLearningResponseDTO> GetCourseLearningAsync(string courseId, string userId)
        {
            if (string.IsNullOrEmpty(courseId))
            {
                return null;
            }

            var obj = await _context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Quizzes)
                .Include(c => c.CourseEnrollments)
                .Where(x => x.CourseId == courseId && x.Status == (int)CourseStatus.Published)
                .Select(course => new CourseLearningResponseDTO
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    Modules = course.Modules
                                    .Where(m => m.Status == (int)ModuleStatus.Active)
                                    .OrderBy(m => m.ModuleNumber)
                                    .Select(m => new ModuleResponseDTO
                                    {
                                        ModuleId = m.ModuleId,
                                        ModuleName = m.ModuleName,
                                        ModuleNumber = m.ModuleNumber,
                                        Lessons = m.Lessons
                                            .Where(l => l.Status == (int)LessonStatus.Active)
                                            .Select(l => new LessonResponseDTO
                                            {
                                                LessonId = l.LessonId,
                                                LessonName = l.LessonName,
                                                LessonNumber = l.LessonNumber,
                                                urlVideo = l.LessonVideo,
                                                Duration = l.Duration
                                            })
                                            .OrderBy(l => l.LessonNumber)
                                            .ToList(),
                                        Quizzes = m.Quizzes
                                                    .Where(q => q.Status == (int)QuizStatus.Active)
                                                    .Select(q => new QuizResponseDTO
                                                    {
                                                        QuizId = q.QuizId,
                                                        QuizName = q.QuizName,
                                                        QuizTime = q.QuizTime,
                                                        TotalQuestions = q.TotalQuestions,
                                                        PassScore = q.PassScore
                                                    })
                                                    .ToList()
                                    })
                                    .ToList(),
                    LessonQuantity = course.Modules
                                            .Where(m => m.Status == (int)ModuleStatus.Active)
                                            .SelectMany(m => m.Lessons)
                                            .Where(l => l.Status == (int)LessonStatus.Active)
                                            .Count(),
                    Progress = course.CourseEnrollments
                                    .Where(e => e.UserId == userId)
                                    .Select(e => e.Progress)
                                    .FirstOrDefault()
                }).FirstOrDefaultAsync();
            obj.LessonIdCompleted = _context.LessonProgresses
                                            .Where(lp => lp.UserId == userId)
                                            .Select(lp => lp.LessonId)
                                            .ToList();
            return obj;
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
            // Lấy courseId từ lesson
            var lesson = _context.Lessons
                .Include(l => l.Module)
                .ThenInclude(m => m.Course)
                .FirstOrDefault(l => l.LessonId == lessonId);

            // Cập nhật Progress trong CourseEnrollment
            UpdateCourseProgress(userId, lesson.Module.Course.CourseId);

            _context.SaveChanges();
        }

        public async Task<bool> CheckEnrollmentAsync(string userId, string courseId)
        {
            var result = await _context.CourseEnrollments.FirstOrDefaultAsync(c => c.UserId.Equals(userId) && c.CourseId.Equals(courseId));
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateQuizProgressAsync(string userId, long quizId)
        {

            // Lấy thông tin quiz để biết module và course
            var quiz = await _context.Quizzes
                .Include(q => q.Module)
                .ThenInclude(m => m.Course)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null) return false;

            // Cập nhật Progress trong CourseEnrollment
            UpdateCourseProgress(userId, quiz.Module.CourseId);
            return true;
        }

        /// <summary>
        /// Cập nhật Progress trong CourseEnrollment dựa trên lesson hoặc quiz hoàn thành
        /// </summary>
        private void UpdateCourseProgress(string userId, string courseId)
        {
            var enrollment = _context.CourseEnrollments
                .FirstOrDefault(e => e.UserId.Equals(userId) && e.CourseId.Equals(courseId));

            if (enrollment == null) return;

            // Tính toán tiến độ mới
            var newProgress = CalculateCourseProgress(userId, courseId);
            enrollment.Progress = newProgress;
            _context.SaveChanges();
        }

        /// <summary>
        /// Tính toán tiến độ course dựa trên lesson và quiz đã hoàn thành
        /// </summary>
        private int CalculateCourseProgress(string userId, string courseId)
        {
            // Lấy thông tin course với modules, lessons và quizzes
            var course = _context.Courses
                .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
                .Include(c => c.Modules)
                .ThenInclude(m => m.Quizzes)
                .FirstOrDefault(c => c.CourseId == courseId);

            if (course == null) return 0;

            // Lấy danh sách lesson và quiz đã hoàn thành
            var completedLessons = _context.LessonProgresses
                .Where(lp => lp.UserId == userId && lp.IsCompleted == true)
                .Select(lp => lp.LessonId)
                .ToList();

            var completedQuizzes = _context.UserQuizResults
                .Where(uqr => uqr.UserId == userId)
                .Select(uqr => uqr.QuizId)
                .ToList();

            // Tính tổng số lesson và quiz trong course
            var totalLessons = course.Modules
                .Where(m => m.Status == (int)ModuleStatus.Active)
                .SelectMany(m => m.Lessons)
                .Where(l => l.Status == (int)LessonStatus.Active)
                .Count();

            var totalQuizzes = course.Modules
                .Where(m => m.Status == (int)ModuleStatus.Active)
                .SelectMany(m => m.Quizzes)
                .Where(q => q.Status == (int)QuizStatus.Active)
                .Count();

            // Tính số lesson và quiz đã hoàn thành trong course này
            var courseLessons = course.Modules
                .Where(m => m.Status == (int)ModuleStatus.Active)
                .SelectMany(m => m.Lessons)
                .Where(l => l.Status == (int)LessonStatus.Active)
                .Select(l => l.LessonId)
                .ToList();

            var courseQuizzes = course.Modules
                .Where(m => m.Status == (int)ModuleStatus.Active)
                .SelectMany(m => m.Quizzes)
                .Where(q => q.Status == (int)QuizStatus.Active)
                .Select(q => q.QuizId)
                .ToList();

            var completedLessonsInCourse = completedLessons
                .Intersect(courseLessons)
                .Count();

            var completedQuizzesInCourse = completedQuizzes
                .Intersect(courseQuizzes)
                .Count();

            // Tính tổng số item cần hoàn thành
            var totalItems = totalLessons + totalQuizzes;

            if (totalItems == 0) return 100; // Nếu không có lesson/quiz nào thì coi như hoàn thành

            // Tính tiến độ
            var completedItems = completedLessonsInCourse + completedQuizzesInCourse;
            var progress = (int)(Convert.ToDouble(completedItems) / totalItems * 100);

            return progress;
        }

        public async Task<int> GetProgressAsync(string userId, string courseId)
        {
            var enrollment = await _context.CourseEnrollments
                .FirstOrDefaultAsync(c => c.UserId.Equals(userId) && c.CourseId.Equals(courseId));

            return enrollment?.Progress ?? 0;
        }
    }
}
