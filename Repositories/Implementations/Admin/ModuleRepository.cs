using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.ModuleDto;
using Online_Learning.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Online_Learning.Repositories.Interfaces.Admin;
using Online_Learning.Constants;


namespace Online_Learning.Repositories.Implementations.Admin
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly OnlineLearningContext _context;
        public ModuleRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModuleResponseDto>> GetModulesAsync(string? courseId, int page, int pageSize)
        {
            var query = _context.Modules
                .Include(m => m.Course)
                .Include(m => m.Lessons)
                .Include(m => m.Quizzes)
                .AsQueryable();
            if (!string.IsNullOrEmpty(courseId))
            {
                query = query.Where(m => m.CourseId == courseId);
            }
            var modules = await query
                .OrderBy(m => m.ModuleNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new ModuleResponseDto
                {
                    ModuleID = m.ModuleId,
                    ModuleName = m.ModuleName,
                    ModuleNumber = m.ModuleNumber,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    Status = m.Status,
                    CourseID = m.CourseId,
                    CourseName = m.Course.CourseName,
                    LessonCount = m.Lessons.Count,
                    QuizCount = m.Quizzes.Count
                })
                .ToListAsync();
            return modules;
        }

        public async Task<ModuleResponseDto?> GetModuleByIdAsync(long id)
        {
            var module = await _context.Modules
                .Include(m => m.Course)
                .Include(m => m.Lessons)
                .Include(m => m.Quizzes)
                .Where(m => m.ModuleId == id)
                .Select(m => new ModuleResponseDto
                {
                    ModuleID = m.ModuleId,
                    ModuleName = m.ModuleName,
                    ModuleNumber = m.ModuleNumber,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    Status = m.Status,
                    CourseID = m.CourseId,
                    CourseName = m.Course.CourseName,
                    LessonCount = m.Lessons.Count,
                    QuizCount = m.Quizzes.Count
                })
                .FirstOrDefaultAsync();
            return module;
        }

        public async Task<int> GetTotalCountAsync(string? courseId)
        {
            var query = _context.Modules.AsQueryable();
            if (!string.IsNullOrEmpty(courseId))
            {
                query = query.Where(m => m.CourseId == courseId);
            }
            return await query.CountAsync();
        }

        public async Task<IEnumerable<ModuleResponseDto>> GetModulesByCourseAsync(string courseId)
        {
            var modules = await _context.Modules
                .Include(m => m.Course)
                .Include(m => m.Lessons)
                .Include(m => m.Quizzes)
                .Where(m => m.CourseId == courseId)
                .OrderBy(m => m.ModuleNumber)
                .Select(m => new ModuleResponseDto
                {
                    ModuleID = m.ModuleId,
                    ModuleName = m.ModuleName,
                    ModuleNumber = m.ModuleNumber,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    Status = m.Status,
                    CourseID = m.CourseId,
                    CourseName = m.Course.CourseName,
                    LessonCount = m.Lessons.Count,
                    QuizCount = m.Quizzes.Count
                })
                .ToListAsync();
            return modules;
        }

        private async Task UpdateCourseStudyTime(string courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Quizzes)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null) return;
            var modules = course.Modules.Where(m => m.Status == 1).ToList();
            int totalLessonMinutes = modules
                .SelectMany(m => m.Lessons)
                .Where(l => l.Status == 1)
                .Sum(l => l.Duration ?? 0);
            int totalQuizMinutes = modules
                .SelectMany(m => m.Quizzes)
                .Where(q => q.Status == 1)
                .Sum(q => q.QuizTime ?? 0);
            int totalMinutes = totalLessonMinutes + totalQuizMinutes;
            course.StudyTime = totalMinutes.ToString();
            await _context.SaveChangesAsync();
        }

        public async Task<ModuleResponseDto> CreateModuleAsync(Module module)
        {
            // Check if course exists
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == module.CourseId && c.DeletedAt == null);
            if (!courseExists)
            {
                throw new InvalidOperationException(Messages.CourseNotFound);
            }
            // Ki?m tra trng s? th? t? module trong course
            bool isDuplicate = await _context.Modules.AnyAsync(m => m.CourseId == module.CourseId && m.ModuleNumber == module.ModuleNumber);
            if (isDuplicate)
                throw new InvalidOperationException(Messages.ModuleOrderExistsInCourse);
            // Auto-generate module number if not provided
            if (module.ModuleNumber <= 0)
            {
                var maxModuleNumber = await _context.Modules
                    .Where(m => m.CourseId == module.CourseId)
                    .MaxAsync(m => (int?)m.ModuleNumber) ?? 0;
                module.ModuleNumber = maxModuleNumber + 1;
            }
            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            await UpdateCourseStudyTime(module.CourseId);
            var response = await GetModuleByIdAsync(module.ModuleId);
            return response!;
        }

        public async Task<bool> UpdateModuleAsync(long id, Module module)
        {
            var existingModule = await _context.Modules.FindAsync(id);
            if (existingModule == null)
            {
                return false;
            }
            // Ki?m tra tr�ng s? th? t? (tr? ch�nh n�)
            bool isDuplicate = await _context.Modules.AnyAsync(m => m.CourseId == existingModule.CourseId && m.ModuleNumber == module.ModuleNumber && m.ModuleId != id);
            if (isDuplicate)
                throw new InvalidOperationException(Messages.ModuleOrderExistsInCourse);
            existingModule.ModuleName = module.ModuleName;
            existingModule.ModuleNumber = module.ModuleNumber;
            existingModule.Status = module.Status;
            existingModule.UpdatedAt = module.UpdatedAt;
            await _context.SaveChangesAsync();
            await UpdateCourseStudyTime(existingModule.CourseId);
            return true;
        }

        public async Task<bool> DeleteModuleAsync(long id)
        {
            var module = await _context.Modules
                .Include(m => m.Lessons)
                .Include(m => m.Quizzes)
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null)
            {
                return false;
            }
            if (module.Lessons.Any() || module.Quizzes.Any())
            {
                throw new InvalidOperationException(Messages.CannotDeleteModuleWithChildren);
            }
            string courseId = module.CourseId;
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            await UpdateCourseStudyTime(courseId);
            return true;
        }
    }
}