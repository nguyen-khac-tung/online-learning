using Online_Learning.Models.DTOs.Request.Admin.ModuleDto;
using Online_Learning.Models.DTOs.Response.Admin.ModuleDto;
using Online_Learning.Repositories.Interfaces.Admin;
using Online_Learning.Services.Interfaces.Admin;
using Online_Learning.Models.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Online_Learning.Services.Implementations.Admin
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ILogger<ModuleService> _logger;

        public ModuleService(IModuleRepository moduleRepository, ILogger<ModuleService> logger)
        {
            _moduleRepository = moduleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ModuleResponseDto>> GetModulesAsync(string? courseId, int page, int pageSize)
        {
            return await _moduleRepository.GetModulesAsync(courseId, page, pageSize);
        }

        public async Task<ModuleResponseDto?> GetModuleByIdAsync(long id)
        {
            return await _moduleRepository.GetModuleByIdAsync(id);
        }

        public async Task<int> GetTotalCountAsync(string? courseId)
        {
            return await _moduleRepository.GetTotalCountAsync(courseId);
        }

        public async Task<IEnumerable<ModuleResponseDto>> GetModulesByCourseAsync(string courseId)
        {
            return await _moduleRepository.GetModulesByCourseAsync(courseId);
        }

        public async Task<ModuleResponseDto> CreateModuleAsync(ModuleCreateDto moduleDto)
        {
            // Logic kiểm tra và sinh số thứ tự module nên chuyển sang repository nếu cần
            var module = new Module
            {
                ModuleName = moduleDto.ModuleName,
                ModuleNumber = moduleDto.ModuleNumber,
                CourseId = moduleDto.CourseID,
                CreatedAt = DateTime.UtcNow,
                Status = moduleDto.status
            };
            return await _moduleRepository.CreateModuleAsync(module);
        }

        public async Task<bool> UpdateModuleAsync(long id, ModuleUpdateDto moduleDto)
        {
            var module = new Module
            {
                ModuleName = moduleDto.ModuleName,
                ModuleNumber = moduleDto.ModuleNumber,
                Status = moduleDto.Status,
                UpdatedAt = DateTime.UtcNow
            };
            return await _moduleRepository.UpdateModuleAsync(id, module);
        }

        public async Task<bool> DeleteModuleAsync(long id)
        {
            return await _moduleRepository.DeleteModuleAsync(id);
        }
    }
} 