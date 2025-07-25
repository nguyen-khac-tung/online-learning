using Online_Learning.Models.DTOs.Request.Admin.Course;
using Online_Learning.Models.DTOs.Response.Admin.Course;
using Online_Learning.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Online_Learning.Services.Interfaces.Admin;
using Online_Learning.Repositories.Interfaces.Admin;
using Online_Learning.Constants;

namespace Online_Learning.Services.Implementations.Admin
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IFileService _fileService;
        private readonly ILogger<CourseService> _logger;

        public CourseService(ICourseRepository courseRepository, IFileService fileService, ILogger<CourseService> logger)
        {
            _courseRepository = courseRepository;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<IEnumerable<CourseResponseDto>> GetCoursesAsync(int page, int pageSize, string? search, int? status)
        {
            return await _courseRepository.GetCoursesAsync(page, pageSize, search, status);
        }

        public async Task<CourseResponseDto?> GetCourseByIdAsync(string id)
        {
            return await _courseRepository.GetCourseByIdAsync(id);
        }

        public async Task<int> GetTotalCountAsync(string? search, int? status)
        {
            return await _courseRepository.GetTotalCountAsync(search, status);
        }

        public async Task<CourseResponseDto> CreateCourseAsync(CourseCreateDto courseDto)
        {
            if (courseDto.AttachmentFiles?.Count > 1)
            {
                throw new InvalidOperationException(Messages.CannotUploadMoreThanOneImageV2);
            }
            var course = new Course
            {
                CourseId = Guid.NewGuid().ToString(),
                CourseName = courseDto.CourseName,
                Description = courseDto.Description,
                Creator = courseDto.Creator,
                StudyTime = courseDto.StudyTime,
                LevelId = courseDto.LevelID,
                LanguageId = courseDto.LanguageID,
                Status = courseDto.Status,
                CreatedAt = DateTime.UtcNow
            };
            var (createdCourse, uploadedFiles, coursePrice) = await _courseRepository.CreateCourseAsync(
                course,
                courseDto.CategoryIDs,
                courseDto.Price,
                courseDto.AttachmentFiles,
                _fileService
            );
            var response = new CourseResponseDto
            {
                CourseID = createdCourse.CourseId,
                CourseName = createdCourse.CourseName,
                Description = createdCourse.Description,
                Creator = createdCourse.Creator,
                Status = createdCourse.Status,
                CreatedAt = createdCourse.CreatedAt,
                StudyTime = createdCourse.StudyTime,
                LevelID = createdCourse.LevelId,
                LanguageID = createdCourse.LanguageId,
                ImageUrls = uploadedFiles,
                ModuleCount = 0,
                CurrentPrice = courseDto.Price,
                PriceHistory = coursePrice != null ? new List<CoursePriceResponseDto>
                {
                    new CoursePriceResponseDto
                    {
                        CoursePriceID = coursePrice.CoursePriceId,
                        CourseID = createdCourse.CourseId,
                        CourseName = createdCourse.CourseName,
                        Price = coursePrice.Price,
                        CreateAt = coursePrice.CreateAt
                    }
                } : new List<CoursePriceResponseDto>(),
                CategoryIDs = courseDto.CategoryIDs ?? new List<int>()
            };
            return response;
        }

        public async Task<bool> UpdateCourseAsync(string id, CourseUpdateDto courseDto)
        {
            return await _courseRepository.UpdateCourseAsync(id, courseDto, _fileService);
        }

        public async Task<bool> DeleteCourseAsync(string id)
        {
            return await _courseRepository.DeleteCourseAsync(id, _fileService);
        }
	}
}
