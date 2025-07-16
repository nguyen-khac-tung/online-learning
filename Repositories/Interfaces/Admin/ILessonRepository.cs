using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.LessonDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Repositories.Interfaces.Admin
{
    public interface ILessonRepository
    {
        Task<IEnumerable<LessonResponseDto>> GetLessonsAsync(long? moduleId, int page, int pageSize);
        Task<LessonResponseDto?> GetLessonByIdAsync(long id);
        Task<int> GetTotalCountAsync(long? moduleId);
        Task<IEnumerable<LessonResponseDto>> GetLessonsByModuleAsync(long moduleId);
        Task<LessonResponseDto> CreateLessonAsync(Lesson lesson);
        Task<bool> UpdateLessonAsync(long id, Lesson lesson);
        Task<bool> DeleteLessonAsync(long id);
        // Có thể bổ sung các method đặc thù khác nếu cần
    }
} 