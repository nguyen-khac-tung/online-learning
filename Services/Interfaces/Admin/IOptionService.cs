using Online_Learning.Models.DTOs.Request.Admin.OptionDto;
using Online_Learning.Models.DTOs.Response.Admin.OptionDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Services.Interfaces.Admin
{
    public interface IOptionService
    {
        Task<IEnumerable<OptionResponseDto>> GetOptionsByQuestionAsync(long questionId);
        Task<OptionResponseDto?> GetOptionByIdAsync(long id);
        Task<OptionResponseDto> CreateOptionAsync(long questionId, OptionCreateDto optionDto);
        Task<bool> UpdateOptionAsync(long id, OptionUpdateDto optionDto);
        Task<bool> DeleteOptionAsync(long id);
    }
} 