using Online_Learning.Models.DTOs.Request.Admin.QuestionDto;
using Online_Learning.Models.DTOs.Response.Admin.QuestionDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Services.Interfaces.Admin
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionResponseDto>> GetQuestionsByQuizAsync(long quizId);
        Task<QuestionResponseDto?> GetQuestionByIdAsync(long id);
        Task<QuestionResponseDto> CreateQuestionAsync(long quizId, QuestionCreateDto questionDto);
        Task<bool> UpdateQuestionAsync(long id, QuestionUpdateDto questionDto);
        Task<bool> DeleteQuestionAsync(long id);
    }
} 