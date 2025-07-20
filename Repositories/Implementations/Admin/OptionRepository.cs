using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.OptionDto;
using Online_Learning.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System;
using Online_Learning.Repositories.Interfaces.Admin;

namespace Online_Learning.Repositories.Implementations.Admin
{
    public class OptionRepository : IOptionRepository
    {
        private readonly OnlineLearningContext _context;
        public OptionRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OptionResponseDto>> GetOptionsByQuestionAsync(long questionId)
        {
            var options = await _context.Options
                .Where(o => o.QuestionId == questionId)
                .Select(o => new OptionResponseDto
                {
                    OptionID = o.OptionId,
                    QuestionID = o.QuestionId,
                    Content = o.Content,
                    IsCorrect = o.IsCorrect,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    Status = o.Status
                })
                .ToListAsync();
            return options;
        }

        public async Task<OptionResponseDto?> GetOptionByIdAsync(long id)
        {
            var option = await _context.Options
                .Where(o => o.OptionId == id)
                .Select(o => new OptionResponseDto
                {
                    OptionID = o.OptionId,
                    QuestionID = o.QuestionId,
                    Content = o.Content,
                    IsCorrect = o.IsCorrect,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    Status = o.Status
                })
                .FirstOrDefaultAsync();
            return option;
        }

        public async Task<OptionResponseDto> CreateOptionAsync(Option option)
        {
            _context.Options.Add(option);
            await _context.SaveChangesAsync();
            var response = await GetOptionByIdAsync(option.OptionId);
            return response!;
        }

        public async Task<bool> UpdateOptionAsync(long id, Option option)
        {
            var existingOption = await _context.Options.FindAsync(id);
            if (existingOption == null)
            {
                return false;
            }
            existingOption.Content = option.Content;
            existingOption.IsCorrect = option.IsCorrect;
            existingOption.Status = option.Status;
            existingOption.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOptionAsync(long id)
        {
            var option = await _context.Options.FindAsync(id);
            if (option == null)
            {
                return false;
            }
            _context.Options.Remove(option);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 