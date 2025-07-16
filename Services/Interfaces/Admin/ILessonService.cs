using Online_Learning.Models.DTOs.Request.Admin.LessonDto;
using Online_Learning.Models.DTOs.Response.Admin.LessonDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Services.Interfaces.Admin
{
    public interface ILessonService
    {
        Task<IEnumerable<LessonResponseDto>> GetLessonsAsync(long? moduleId, int page, int pageSize);
        Task<LessonResponseDto?> GetLessonByIdAsync(long id);
        Task<LessonResponseDto> CreateLessonAsync(LessonCreateDto lessonDto);
        Task<bool> UpdateLessonAsync(long id, LessonUpdateDto lessonDto);
        Task<bool> DeleteLessonAsync(long id);
        Task<int> GetTotalCountAsync(long? moduleId);
        Task<IEnumerable<LessonResponseDto>> GetLessonsByModuleAsync(long moduleId);
    }
} 