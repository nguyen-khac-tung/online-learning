using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Response.Admin.QuizDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Repositories.Interfaces.Admin
{
    public interface IQuizRepository
    {
        Task<IEnumerable<QuizResponseDto>> GetQuizzesAsync(long? moduleId, int page, int pageSize);
        Task<QuizResponseDto?> GetQuizByIdAsync(long id);
        Task<int> GetTotalCountAsync(long? moduleId);
        Task<IEnumerable<QuizResponseDto>> GetQuizzesByModuleAsync(long moduleId);
        Task<QuizResponseDto> CreateQuizAsync(Quiz quiz);
        Task<bool> UpdateQuizAsync(long id, Quiz quiz);
        Task<bool> DeleteQuizAsync(long id);
    }
} 