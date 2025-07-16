using Online_Learning.Models.DTOs.Request.Admin.LessonDto;
using Online_Learning.Models.DTOs.Response.Admin.LessonDto;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Interfaces;
using Online_Learning.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Online_Learning.Services.Interfaces.Admin;
using Online_Learning.Repositories.Interfaces.Admin;

namespace Online_Learning.Services.Implementations.Admin
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ILogger<LessonService> _logger;

        public LessonService(ILessonRepository lessonRepository, ILogger<LessonService> logger)
        {
            _lessonRepository = lessonRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<LessonResponseDto>> GetLessonsAsync(long? moduleId, int page, int pageSize)
        {
            return await _lessonRepository.GetLessonsAsync(moduleId, page, pageSize);
        }

        public async Task<LessonResponseDto?> GetLessonByIdAsync(long id)
        {
            return await _lessonRepository.GetLessonByIdAsync(id);
        }

        public async Task<int> GetTotalCountAsync(long? moduleId)
        {
            return await _lessonRepository.GetTotalCountAsync(moduleId);
        }

        public async Task<IEnumerable<LessonResponseDto>> GetLessonsByModuleAsync(long moduleId)
        {
            return await _lessonRepository.GetLessonsByModuleAsync(moduleId);
        }

        public async Task<LessonResponseDto> CreateLessonAsync(LessonCreateDto lessonDto)
        {
            var lesson = new Lesson
            {
                ModuleId = lessonDto.ModuleID,
                LessonNumber = lessonDto.LessonNumber,
                LessonName = lessonDto.LessonName,
                LessonContent = lessonDto.LessonContent,
                LessonVideo = lessonDto.LessonVideo,
                Duration = lessonDto.Duration,
                CreatedAt = DateTime.UtcNow,
                Status = lessonDto.Status
            };
            return await _lessonRepository.CreateLessonAsync(lesson);
        }

        public async Task<bool> UpdateLessonAsync(long id, LessonUpdateDto lessonDto)
        {
            var lesson = new Lesson
            {
                LessonNumber = lessonDto.LessonNumber,
                LessonName = lessonDto.LessonName,
                LessonContent = lessonDto.LessonContent,
                LessonVideo = lessonDto.LessonVideo,
                Duration = lessonDto.Duration,
                Status = lessonDto.Status,
                UpdatedAt = DateTime.UtcNow
            };
            return await _lessonRepository.UpdateLessonAsync(id, lesson);
        }

        public async Task<bool> DeleteLessonAsync(long id)
        {
            return await _lessonRepository.DeleteLessonAsync(id);
        }
    }
} 