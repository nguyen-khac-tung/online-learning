using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Constants.Enums;

namespace Online_Learning.Repositories.Implementations
{
    public class QuizzRepository : IQuizzRepository
    {
        private readonly OnlineLearningContext _context;

        public QuizzRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<Quiz?> GetQuizByIdAsync(long quizId)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(qu => qu.Options)
                .FirstOrDefaultAsync(q => q.QuizId == quizId && q.Status == (int)QuizStatus.Active);
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesByModuleAsync(long moduleId)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(qu => qu.Options)
                .Where(q => q.ModuleId == moduleId && q.Status == (int)QuizStatus.Active)
                .ToListAsync();
        }

        public async Task<UserQuizResult?> GetUserQuizResultAsync(string userId, long quizId)
        {
            return await _context.UserQuizResults
                .Include(uqr => uqr.Quiz)
                .FirstOrDefaultAsync(uqr => uqr.UserId == userId && uqr.QuizId == quizId);
        }

        public async Task<UserQuizResult> CreateUserQuizResultAsync(UserQuizResult userQuizResult)
        {
            _context.UserQuizResults.Add(userQuizResult);
            await _context.SaveChangesAsync();
            return userQuizResult;
        }

        public async Task<IEnumerable<UserQuizResult>> GetUserQuizResultsAsync(string userId)
        {
            return await _context.UserQuizResults
                .Include(uqr => uqr.Quiz)
                .Where(uqr => uqr.UserId == userId)
                .OrderByDescending(uqr => uqr.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsQuizCompletedAsync(string userId, long quizId)
        {
            return await _context.UserQuizResults
                .AnyAsync(uqr => uqr.UserId == userId && uqr.QuizId == quizId);
        }

        public async Task<Quiz?> GetQuizWithQuestionsAndOptionsAsync(long quizId)
        {
            return await _context.Quizzes
                .Include(q => q.Questions.Where(qu => qu.Status == (int)QuestionStatus.Active))
                .ThenInclude(qu => qu.Options.Where(o => o.Status == (int)OptionStatus.Active))
                .FirstOrDefaultAsync(q => q.QuizId == quizId && q.Status == (int)QuizStatus.Active);
        }

        public async Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(long quizId)
        {
            return await _context.Questions
                .Include(q => q.Options)
                .Where(q => q.QuizId == quizId && q.Status == (int)QuestionStatus.Active)
                .OrderBy(q => q.QuestionNum)
                .ToListAsync();
        }

        public async Task<IEnumerable<Option>> GetOptionsByQuestionIdAsync(long questionId)
        {
            return await _context.Options
                .Where(o => o.QuestionId == questionId && o.Status == (int)OptionStatus.Active)
                .ToListAsync();
        }

        public async Task<bool> ValidateQuizSubmissionAsync(QuizRequestDto request, long quizId)
        {
            var quiz = await GetQuizByIdAsync(quizId);
            if (quiz == null) return false;

            var questions = await GetQuestionsByQuizIdAsync(quizId);
            var totalQuestions = questions.Count();
            var submittedQuestions = request.Answers.Count;

            // Kiểm tra số câu trả lời phải bằng số câu hỏi
            if (submittedQuestions != totalQuestions) return false;

            // Kiểm tra tất cả câu hỏi đều có câu trả lời
            var questionIds = questions.Select(q => q.QuestionId).ToHashSet();
            var submittedQuestionIds = request.Answers.Select(a => a.QuestionId).ToHashSet();

            if (!questionIds.SetEquals(submittedQuestionIds)) return false;

            return true;
        }
    }
}
