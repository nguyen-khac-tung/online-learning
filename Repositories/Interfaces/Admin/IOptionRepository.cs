using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.OptionDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Repositories.Interfaces.Admin
{
    public interface IOptionRepository 
    {
        Task<IEnumerable<OptionResponseDto>> GetOptionsByQuestionAsync(long questionId);
        Task<OptionResponseDto?> GetOptionByIdAsync(long id);
        Task<OptionResponseDto> CreateOptionAsync(Option option);
        Task<bool> UpdateOptionAsync(long id, Option option);
        Task<bool> DeleteOptionAsync(long id);
    }
} 