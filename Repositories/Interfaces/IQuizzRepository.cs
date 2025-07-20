using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.Entities;

namespace Online_Learning.Repositories.Interfaces
{
    public interface IQuizzRepository
    {
        Task<Quiz?> GetQuizByIdAsync(long quizId);
        Task<IEnumerable<Quiz>> GetQuizzesByModuleAsync(long moduleId);
        Task<UserQuizResult?> GetUserQuizResultAsync(string userId, long quizId);
        Task<UserQuizResult> CreateUserQuizResultAsync(UserQuizResult userQuizResult);
        Task UpdateUserQuizResultAsync(UserQuizResult userQuizResult);
        Task<IEnumerable<UserQuizResult>> GetUserQuizResultsAsync(string userId);
        Task<bool> IsQuizCompletedAsync(string userId, long quizId);
        Task<Quiz?> GetQuizWithQuestionsAndOptionsAsync(long quizId);
        Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(long quizId);
        Task<IEnumerable<Option>> GetOptionsByQuestionIdAsync(long questionId);
        Task<bool> ValidateQuizSubmissionAsync(QuizRequestDto request, long quizId);
    }
}
