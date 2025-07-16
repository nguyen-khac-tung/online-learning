using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.ModuleDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Repositories.Interfaces.Admin
{
    public interface IModuleRepository
    {
        Task<IEnumerable<ModuleResponseDto>> GetModulesAsync(string? courseId, int page, int pageSize);
        Task<ModuleResponseDto?> GetModuleByIdAsync(long id);
        Task<int> GetTotalCountAsync(string? courseId);
        Task<IEnumerable<ModuleResponseDto>> GetModulesByCourseAsync(string courseId);
        Task<ModuleResponseDto> CreateModuleAsync(Module module);
        Task<bool> UpdateModuleAsync(long id, Module module);
        Task<bool> DeleteModuleAsync(long id);
        // Có thể bổ sung các method đặc thù khác nếu cần
    }
} 