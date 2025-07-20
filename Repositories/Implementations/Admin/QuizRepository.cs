using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.QuizDto;
using Online_Learning.Models.DTOs.Response.Admin.QuestionDto;
using Online_Learning.Models.DTOs.Response.Admin.OptionDto;
using Online_Learning.Repositories.Interfaces;
using System.Collections.Generic;
using System;
using Online_Learning.Repositories.Interfaces.Admin;

namespace Online_Learning.Repositories.Implementations.Admin
{
    public class QuizRepository : IQuizRepository
    {
        private readonly OnlineLearningContext _context;
        public QuizRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuizResponseDto>> GetQuizzesAsync(long? moduleId, int page, int pageSize)
        {
            var query = _context.Quizzes
                .Include(q => q.Module)
                .Include(q => q.Questions).ThenInclude(qu => qu.Options)
                .AsQueryable();
            if (moduleId.HasValue)
            {
                query = query.Where(q => q.ModuleId == moduleId.Value);
            }
            var quizzes = await query
                .OrderByDescending(q => q.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new QuizResponseDto
                {
                    QuizID = q.QuizId,
                    ModuleID = q.ModuleId,
                    ModuleName = q.Module.ModuleName,
                    QuizName = q.QuizName,
                    QuizTime = q.QuizTime,
                    TotalQuestions = q.Questions.Count(),
                    PassScore = q.PassScore,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    Status = q.Status,
                    Questions = q.Questions.Select(qu => new QuestionResponseDto
                    {
                        QuestionID = qu.QuestionId,
                        QuizID = qu.QuizId,
                        QuestionNum = qu.QuestionNum,
                        Content = qu.Content,
                        Type = qu.Type,
                        CreatedAt = qu.CreatedAt,
                        UpdatedAt = qu.UpdatedAt,
                        Status = qu.Status,
                        Options = qu.Options.Select(o => new OptionResponseDto
                        {
                            OptionID = o.OptionId,
                            QuestionID = o.QuestionId,
                            Content = o.Content,
                            IsCorrect = o.IsCorrect,
                            CreatedAt = o.CreatedAt,
                            UpdatedAt = o.UpdatedAt,
                            Status = o.Status
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();
            return quizzes;
        }

        public async Task<QuizResponseDto?> GetQuizByIdAsync(long id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Module)
                .Include(q => q.Questions).ThenInclude(qu => qu.Options)
                .Where(q => q.QuizId == id)
                .Select(q => new QuizResponseDto
                {
                    QuizID = q.QuizId,
                    ModuleID = q.ModuleId,
                    ModuleName = q.Module.ModuleName,
                    QuizName = q.QuizName,
                    QuizTime = q.QuizTime,
                    TotalQuestions = q.TotalQuestions,
                    PassScore = q.PassScore,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    Status = q.Status,
                    Questions = q.Questions.OrderBy(qu => qu.QuestionNum).Select(qu => new QuestionResponseDto
                    {
                        QuestionID = qu.QuestionId,
                        QuizID = qu.QuizId,
                        QuestionNum = qu.QuestionNum,
                        Content = qu.Content,
                        Type = qu.Type,
                        CreatedAt = qu.CreatedAt,
                        UpdatedAt = qu.UpdatedAt,
                        Status = qu.Status,
                        Options = qu.Options.Select(o => new OptionResponseDto
                        {
                            OptionID = o.OptionId,
                            QuestionID = o.QuestionId,
                            Content = o.Content,
                            IsCorrect = o.IsCorrect,
                            CreatedAt = o.CreatedAt,
                            UpdatedAt = o.UpdatedAt,
                            Status = o.Status
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync();
            return quiz;
        }

        public async Task<int> GetTotalCountAsync(long? moduleId)
        {
            var query = _context.Quizzes.AsQueryable();
            if (moduleId.HasValue)
            {
                query = query.Where(q => q.ModuleId == moduleId.Value);
            }
            return await query.CountAsync();
        }

        public async Task<IEnumerable<QuizResponseDto>> GetQuizzesByModuleAsync(long moduleId)
        {
            var quizzes = await _context.Quizzes
                .Include(q => q.Module)
                .Include(q => q.Questions).ThenInclude(qu => qu.Options)
                .Where(q => q.ModuleId == moduleId)
                .OrderByDescending(q => q.CreatedAt)
                .Select(q => new QuizResponseDto
                {
                    QuizID = q.QuizId,
                    ModuleID = q.ModuleId,
                    ModuleName = q.Module.ModuleName,
                    QuizName = q.QuizName,
                    QuizTime = q.QuizTime,
                    TotalQuestions = q.TotalQuestions,
                    PassScore = q.PassScore,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    Status = q.Status,
                    Questions = q.Questions.Select(qu => new QuestionResponseDto
                    {
                        QuestionID = qu.QuestionId,
                        QuizID = qu.QuizId,
                        QuestionNum = qu.QuestionNum,
                        Content = qu.Content,
                        Type = qu.Type,
                        CreatedAt = qu.CreatedAt,
                        UpdatedAt = qu.UpdatedAt,
                        Status = qu.Status,
                        Options = qu.Options.Select(o => new OptionResponseDto
                        {
                            OptionID = o.OptionId,
                            QuestionID = o.QuestionId,
                            Content = o.Content,
                            IsCorrect = o.IsCorrect,
                            CreatedAt = o.CreatedAt,
                            UpdatedAt = o.UpdatedAt,
                            Status = o.Status
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();
            return quizzes;
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

        public async Task<QuizResponseDto> CreateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            await UpdateCourseStudyTime(quiz.ModuleId);
            var response = await GetQuizByIdAsync(quiz.QuizId);
            return response!;
        }

        public async Task<bool> UpdateQuizAsync(long id, Quiz quiz)
        {
            var existingQuiz = await _context.Quizzes.FindAsync(id);
            if (existingQuiz == null)
            {
                return false;
            }
            existingQuiz.QuizName = quiz.QuizName;
            existingQuiz.QuizTime = quiz.QuizTime;
            existingQuiz.PassScore = quiz.PassScore;
            existingQuiz.Status = quiz.Status;
            existingQuiz.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await UpdateCourseStudyTime(existingQuiz.ModuleId);
            return true;
        }

        public async Task<bool> DeleteQuizAsync(long id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return false;
            }
            long moduleId = quiz.ModuleId;
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            await UpdateCourseStudyTime(moduleId);
            return true;
        }
    }
} 