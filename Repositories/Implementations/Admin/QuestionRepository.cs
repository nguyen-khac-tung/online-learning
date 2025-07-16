using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.QuestionDto;
using Online_Learning.Models.DTOs.Response.Admin.OptionDto;
using Online_Learning.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Online_Learning.Repositories.Interfaces.Admin;

namespace Online_Learning.Repositories.Implementations.Admin
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly OnlineLearningContext _context;
        public QuestionRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuestionResponseDto>> GetQuestionsByQuizAsync(long quizId)
        {
            var questions = await _context.Questions
                .Include(q => q.Options)
                .Where(q => q.QuizId == quizId)
                .OrderBy(q => q.QuestionNum)
                .Select(q => new QuestionResponseDto
                {
                    QuestionID = q.QuestionId,
                    QuizID = q.QuizId,
                    QuestionNum = q.QuestionNum,
                    Content = q.Content,
                    Type = q.Type,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    Status = q.Status,
                    Options = q.Options.Select(o => new OptionResponseDto
                    {
                        OptionID = o.OptionId,
                        QuestionID = o.QuestionId,
                        Content = o.Content,
                        IsCorrect = o.IsCorrect,
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt,
                        Status = o.Status
                    }).ToList()
                })
                .ToListAsync();
            return questions;
        }

        public async Task<QuestionResponseDto?> GetQuestionByIdAsync(long id)
        {
            var question = await _context.Questions
                .Include(q => q.Options)
                .Where(q => q.QuestionId == id)
                .Select(q => new QuestionResponseDto
                {
                    QuestionID = q.QuestionId,
                    QuizID = q.QuizId,
                    QuestionNum = q.QuestionNum,
                    Content = q.Content,
                    Type = q.Type,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    Status = q.Status,
                    Options = q.Options.Select(o => new OptionResponseDto
                    {
                        OptionID = o.OptionId,
                        QuestionID = o.QuestionId,
                        Content = o.Content,
                        IsCorrect = o.IsCorrect,
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt,
                        Status = o.Status
                    }).ToList()
                })
                .FirstOrDefaultAsync();
            return question;
        }

        public async Task<QuestionResponseDto> CreateQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            if (question.Options != null && question.Options.Count > 0)
            {
                foreach (var option in question.Options)
                {
                    _context.Options.Add(option);
                }
            }
            await _context.SaveChangesAsync();
            var response = await GetQuestionByIdAsync(question.QuestionId);
            return response!;
        }

        public async Task<bool> UpdateQuestionAsync(long id, Question question)
        {
            var existingQuestion = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.QuestionId == id);
            if (existingQuestion == null)
            {
                return false;
            }
            existingQuestion.QuestionNum = question.QuestionNum;
            existingQuestion.Content = question.Content;
            existingQuestion.Type = question.Type;
            existingQuestion.Status = question.Status;
            existingQuestion.UpdatedAt = DateTime.UtcNow;

            // Update options
            var existingOptions = existingQuestion.Options.ToList();
            var incomingOptions = question.Options ?? new List<Option>();

            // Remove options not in incoming
            var optionsToRemove = existingOptions.Where(eo => !incomingOptions.Any(io => io.OptionId == eo.OptionId && io.OptionId != 0)).ToList();
            _context.Options.RemoveRange(optionsToRemove);

            // Add or update options
            foreach (var incomingOption in incomingOptions)
            {
                if (incomingOption.OptionId == 0)
                {
                    // New option
                    existingQuestion.Options.Add(new Option
                    {
                        Content = incomingOption.Content,
                        IsCorrect = incomingOption.IsCorrect,
                        Status = incomingOption.Status,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    var existingOption = existingOptions.FirstOrDefault(o => o.OptionId == incomingOption.OptionId);
                    if (existingOption != null)
                    {
                        existingOption.Content = incomingOption.Content;
                        existingOption.IsCorrect = incomingOption.IsCorrect;
                        existingOption.Status = incomingOption.Status;
                        existingOption.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteQuestionAsync(long id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return false;
            }
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 