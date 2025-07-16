using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.QuestionDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Repositories.Interfaces.Admin
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<QuestionResponseDto>> GetQuestionsByQuizAsync(long quizId);
        Task<QuestionResponseDto?> GetQuestionByIdAsync(long id);
        Task<QuestionResponseDto> CreateQuestionAsync(Question question);
        Task<bool> UpdateQuestionAsync(long id, Question question);
        Task<bool> DeleteQuestionAsync(long id);
    }
} 