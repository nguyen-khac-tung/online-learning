using Online_Learning.Models.DTOs.Request.Admin.OptionDto;
using Online_Learning.Models.DTOs.Response.Admin.OptionDto;
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
    public class OptionService : IOptionService
    {
        private readonly IOptionRepository _optionRepository;
        private readonly ILogger<OptionService> _logger;

        public OptionService(IOptionRepository optionRepository, ILogger<OptionService> logger)
        {
            _optionRepository = optionRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<OptionResponseDto>> GetOptionsByQuestionAsync(long questionId)
        {
            return await _optionRepository.GetOptionsByQuestionAsync(questionId);
        }

        public async Task<OptionResponseDto?> GetOptionByIdAsync(long id)
        {
            return await _optionRepository.GetOptionByIdAsync(id);
        }

        public async Task<OptionResponseDto> CreateOptionAsync(long questionId, OptionCreateDto optionDto)
        {
            var option = new Option
            {
                QuestionId = questionId,
                Content = optionDto.Content,
                IsCorrect = optionDto.IsCorrect,
                Status = optionDto.Status,
                CreatedAt = DateTime.UtcNow
            };
            return await _optionRepository.CreateOptionAsync(option);
        }

        public async Task<bool> UpdateOptionAsync(long id, OptionUpdateDto optionDto)
        {
            var option = new Option
            {
                Content = optionDto.Content,
                IsCorrect = optionDto.IsCorrect,
                Status = optionDto.Status,
                UpdatedAt = DateTime.UtcNow
            };
            return await _optionRepository.UpdateOptionAsync(id, option);
        }

        public async Task<bool> DeleteOptionAsync(long id)
        {
            return await _optionRepository.DeleteOptionAsync(id);
        }
    }
} 