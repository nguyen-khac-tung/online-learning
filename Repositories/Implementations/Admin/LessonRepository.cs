using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.LessonDto;
using Online_Learning.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Online_Learning.Repositories.Interfaces.Admin;

namespace Online_Learning.Repositories.Implementations.Admin
{
    public class LessonRepository : ILessonRepository
    {
        private readonly OnlineLearningContext _context;
        public LessonRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LessonResponseDto>> GetLessonsAsync(long? moduleId, int page, int pageSize)
        {
            var query = _context.Lessons.Include(l => l.Module).AsQueryable();
            if (moduleId.HasValue)
            {
                query = query.Where(l => l.ModuleId == moduleId.Value);
            }
            var lessons = await query
                .OrderBy(l => l.LessonNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LessonResponseDto
                {
                    LessonID = l.LessonId,
                    ModuleID = l.ModuleId,
                    ModuleName = l.Module.ModuleName,
                    LessonNumber = l.LessonNumber,
                    LessonName = l.LessonName,
                    LessonContent = l.LessonContent,
                    LessonVideo = l.LessonVideo,
                    Duration = l.Duration,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt,
                    Status = l.Status
                })
                .ToListAsync();
            return lessons;
        }

        public async Task<LessonResponseDto?> GetLessonByIdAsync(long id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Module)
                .Where(l => l.LessonId == id)
                .Select(l => new LessonResponseDto
                {
                    LessonID = l.LessonId,
                    ModuleID = l.ModuleId,
                    ModuleName = l.Module.ModuleName,
                    LessonNumber = l.LessonNumber,
                    LessonName = l.LessonName,
                    LessonContent = l.LessonContent,
                    LessonVideo = l.LessonVideo,
                    Duration = l.Duration,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt,
                    Status = l.Status
                })
                .FirstOrDefaultAsync();
            return lesson;
        }

        public async Task<int> GetTotalCountAsync(long? moduleId)
        {
            var query = _context.Lessons.AsQueryable();
            if (moduleId.HasValue)
            {
                query = query.Where(l => l.ModuleId == moduleId.Value);
            }
            return await query.CountAsync();
        }

        public async Task<IEnumerable<LessonResponseDto>> GetLessonsByModuleAsync(long moduleId)
        {
            var lessons = await _context.Lessons
                .Include(l => l.Module)
                .Where(l => l.ModuleId == moduleId)
                .OrderBy(l => l.LessonNumber)
                .Select(l => new LessonResponseDto
                {
                    LessonID = l.LessonId,
                    ModuleID = l.ModuleId,
                    ModuleName = l.Module.ModuleName,
                    LessonNumber = l.LessonNumber,
                    LessonName = l.LessonName,
                    LessonContent = l.LessonContent,
                    LessonVideo = l.LessonVideo,
                    Duration = l.Duration,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt,
                    Status = l.Status
                })
                .ToListAsync();
            return lessons;
        }

        private async Task UpdateCourseStudyTime(long moduleId)
        {
            var module = await _context.Modules
                .Include(m => m.Lessons)
                .Include(m => m.Quizzes)
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId);
            if (module == null || module.Course == null) return;
            var course = module.Course;
            // Lấy tất cả module active của course
            var modules = await _context.Modules
                .Where(m => m.CourseId == course.CourseId && m.Status == 1)
                .Include(m => m.Lessons)
                .Include(m => m.Quizzes)
                .ToListAsync();
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

        public async Task<LessonResponseDto?> CreateLessonAsync(Lesson lesson)
        {
            try
            {
                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();
                await UpdateCourseStudyTime(lesson.ModuleId);
                var response = await GetLessonByIdAsync(lesson.LessonId);
                return response!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] CreateLessonAsync failed: {ex.Message}");
                Console.WriteLine($"[STACKTRACE] {ex.StackTrace}");
                return null; // ho?c throw l?i n?u mu?n x? l bn ngoi
            }
        }


        public async Task<bool> UpdateLessonAsync(long id, Lesson lesson)
        {
            var existingLesson = await _context.Lessons.FindAsync(id);
            if (existingLesson == null)
            {
                return false;
            }
            existingLesson.LessonNumber = lesson.LessonNumber;
            existingLesson.LessonName = lesson.LessonName;
            existingLesson.LessonContent = lesson.LessonContent;
            existingLesson.LessonVideo = lesson.LessonVideo;
            existingLesson.Duration = lesson.Duration;
            existingLesson.Status = lesson.Status;
            existingLesson.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await UpdateCourseStudyTime(existingLesson.ModuleId);
            return true;
        }

        public async Task<bool> DeleteLessonAsync(long id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return false;
            }
            long moduleId = lesson.ModuleId;
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            await UpdateCourseStudyTime(moduleId);
            return true;
        }
    }
} 