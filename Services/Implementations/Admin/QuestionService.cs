using Online_Learning.Models.DTOs.Request.Admin.QuestionDto;
using Online_Learning.Models.DTOs.Response.Admin.QuestionDto;
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
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(IQuestionRepository questionRepository, ILogger<QuestionService> logger)
        {
            _questionRepository = questionRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<QuestionResponseDto>> GetQuestionsByQuizAsync(long quizId)
        {
            return await _questionRepository.GetQuestionsByQuizAsync(quizId);
        }

        public async Task<QuestionResponseDto?> GetQuestionByIdAsync(long id)
        {
            return await _questionRepository.GetQuestionByIdAsync(id);
        }

        public async Task<QuestionResponseDto> CreateQuestionAsync(long quizId, QuestionCreateDto questionDto)
        {
            var question = new Question
            {
                QuizId = quizId,
                QuestionNum = questionDto.QuestionNum,
                Content = questionDto.Content,
                Type = questionDto.Type,
                Status = questionDto.Status,
                CreatedAt = DateTime.UtcNow,
                Options = questionDto.Options.Select(o => new Option
                {
                    Content = o.Content,
                    IsCorrect = o.IsCorrect,
                    Status = o.Status,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };
            return await _questionRepository.CreateQuestionAsync(question);
        }

        public async Task<bool> UpdateQuestionAsync(long id, QuestionUpdateDto questionDto)
        {
            var question = new Question
            {
                QuestionNum = questionDto.QuestionNum,
                Content = questionDto.Content,
                Type = questionDto.Type,
                Status = questionDto.Status,
                UpdatedAt = DateTime.UtcNow,
                Options = questionDto.Options.Select(o => new Option
                {
                    OptionId = o.OptionID,
                    Content = o.Content,
                    IsCorrect = o.IsCorrect,
                    Status = o.Status,
                    UpdatedAt = DateTime.UtcNow
                }).ToList()
            };
            return await _questionRepository.UpdateQuestionAsync(id, question);
        }

        public async Task<bool> DeleteQuestionAsync(long id)
        {
            return await _questionRepository.DeleteQuestionAsync(id);
        }
    }
} 