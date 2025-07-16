using Online_Learning.Models.DTOs.Request.Admin.QuizDto;
using Online_Learning.Models.DTOs.Response.Admin.QuizDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Services.Interfaces.Admin
{
    public interface IQuizService
    {
        Task<IEnumerable<QuizResponseDto>> GetQuizzesAsync(long? moduleId, int page, int pageSize);
        Task<QuizResponseDto?> GetQuizByIdAsync(long id);
        Task<QuizResponseDto> CreateQuizAsync(QuizCreateDto quizDto);
        Task<bool> UpdateQuizAsync(long id, QuizUpdateDto quizDto);
        Task<bool> DeleteQuizAsync(long id);
        Task<int> GetTotalCountAsync(long? moduleId);
        Task<IEnumerable<QuizResponseDto>> GetQuizzesByModuleAsync(long moduleId);
    }
} 