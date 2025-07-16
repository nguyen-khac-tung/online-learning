using Online_Learning.Models.DTOs.Request.Admin.QuizDto;
using Online_Learning.Models.DTOs.Response.Admin.QuizDto;
using Online_Learning.Repositories.Interfaces.Admin;
using Online_Learning.Services.Interfaces.Admin;
using Online_Learning.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Online_Learning.Services.Implementations.Admin
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ILogger<QuizService> _logger;

        public QuizService(IQuizRepository quizRepository, ILogger<QuizService> logger)
        {
            _quizRepository = quizRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<QuizResponseDto>> GetQuizzesAsync(long? moduleId, int page, int pageSize)
        {
            return await _quizRepository.GetQuizzesAsync(moduleId, page, pageSize);
        }

        public async Task<QuizResponseDto?> GetQuizByIdAsync(long id)
        {
            return await _quizRepository.GetQuizByIdAsync(id);
        }

        public async Task<int> GetTotalCountAsync(long? moduleId)
        {
            return await _quizRepository.GetTotalCountAsync(moduleId);
        }

        public async Task<IEnumerable<QuizResponseDto>> GetQuizzesByModuleAsync(long moduleId)
        {
            return await _quizRepository.GetQuizzesByModuleAsync(moduleId);
        }

        public async Task<QuizResponseDto> CreateQuizAsync(QuizCreateDto quizDto)
        {
            var quiz = new Quiz
            {
                ModuleId = quizDto.ModuleID,
                QuizName = quizDto.QuizName,
                QuizTime = quizDto.QuizTime,
                TotalQuestions = quizDto.Questions.Count,
                PassScore = quizDto.PassScore,
                CreatedAt = DateTime.UtcNow,
                Status = 1
            };
            // Note: Questions and Options creation should be handled elsewhere if needed
            return await _quizRepository.CreateQuizAsync(quiz);
        }

        public async Task<bool> UpdateQuizAsync(long id, QuizUpdateDto quizDto)
        {
            var quiz = new Quiz
            {
                QuizName = quizDto.QuizName,
                QuizTime = quizDto.QuizTime,
                PassScore = quizDto.PassScore,
                Status = quizDto.Status,
                UpdatedAt = DateTime.UtcNow
            };
            return await _quizRepository.UpdateQuizAsync(id, quiz);
        }

        public async Task<bool> DeleteQuizAsync(long id)
        {
            return await _quizRepository.DeleteQuizAsync(id);
        }
    }
} 