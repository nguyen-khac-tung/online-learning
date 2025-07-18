using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.User;

namespace Online_Learning.Services.Interfaces
{
    public interface IQuizService
    {
        Task<QuizResponseDTO> GetQuizByIdAsync(long quizId);
        Task<IEnumerable<QuizResponseDTO>> GetQuizzesByModuleAsync(long moduleId);
        Task<QuizResultResponseDTO> SubmitQuizAsync(QuizRequestDto request, string userId);
        Task<QuizResultResponseDTO> GetQuizResultAsync(long quizId, string userId);
        Task<IEnumerable<QuizResultResponseDTO>> GetUserQuizResultsAsync(string userId);
        Task<bool> IsQuizCompletedAsync(long quizId, string userId);
        Task<bool> UpdateQuizProgressAsync(string userId, long quizId);
    }
} 