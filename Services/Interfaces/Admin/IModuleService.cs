using Online_Learning.Models.DTOs.Request.Admin.ModuleDto;
using Online_Learning.Models.DTOs.Response.Admin.ModuleDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Services.Interfaces.Admin
{
    public interface IModuleService
    {
        Task<IEnumerable<ModuleResponseDto>> GetModulesAsync(string? courseId, int page, int pageSize);
        Task<ModuleResponseDto?> GetModuleByIdAsync(long id);
        Task<ModuleResponseDto> CreateModuleAsync(ModuleCreateDto moduleDto);
        Task<bool> UpdateModuleAsync(long id, ModuleUpdateDto moduleDto);
        Task<bool> DeleteModuleAsync(long id);
        Task<int> GetTotalCountAsync(string? courseId);
        Task<IEnumerable<ModuleResponseDto>> GetModulesByCourseAsync(string courseId);
    }
} 