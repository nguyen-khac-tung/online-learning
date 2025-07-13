using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Interfaces;
using Online_Learning.Constants.Enums;

namespace Online_Learning.Services.Implementations
{
    public class QuizService : IQuizService
    {
        private readonly IQuizzRepository _quizRepository;
        private readonly ICourseRepository _courseRepository;

        public QuizService(IQuizzRepository quizRepository, ICourseRepository courseRepository)
        {
            _quizRepository = quizRepository;
            _courseRepository = courseRepository;
        }

        public async Task<QuizResponseDTO> GetQuizByIdAsync(long quizId)
        {
            var quiz = await _quizRepository.GetQuizWithQuestionsAndOptionsAsync(quizId);
            if (quiz == null) return null;

            return new QuizResponseDTO
            {
                QuizId = quiz.QuizId,
                QuizName = quiz.QuizName,
                QuizTime = quiz.QuizTime,
                TotalQuestions = quiz.TotalQuestions,
                PassScore = quiz.PassScore,
                Questions = quiz.Questions.Select(q => new QuestionResponseDTO
                {
                    QuestionId = q.QuestionId,
                    QuestionNum = q.QuestionNum,
                    Content = q.Content,
                    Type = q.Type,
                    Options = q.Options.Select(o => new OptionResponseDTO
                    {
                        OptionId = o.OptionId,
                        Content = o.Content
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<IEnumerable<QuizResponseDTO>> GetQuizzesByModuleAsync(long moduleId)
        {
            var quizzes = await _quizRepository.GetQuizzesByModuleAsync(moduleId);
            return quizzes.Select(q => new QuizResponseDTO
            {
                QuizId = q.QuizId,
                QuizName = q.QuizName,
                QuizTime = q.QuizTime,
                TotalQuestions = q.TotalQuestions,
                PassScore = q.PassScore,
                Questions = q.Questions.Select(qu => new QuestionResponseDTO
                {
                    QuestionId = qu.QuestionId,
                    QuestionNum = qu.QuestionNum,
                    Content = qu.Content,
                    Type = qu.Type,
                    Options = qu.Options.Select(o => new OptionResponseDTO
                    {
                        OptionId = o.OptionId,
                        Content = o.Content
                    }).ToList()
                }).ToList()
            });
        }

        public async Task<QuizResultResponseDTO> SubmitQuizAsync(QuizRequestDto request, string userId)
        {
            // Kiểm tra quiz đã được làm chưa
            var existingResult = await _quizRepository.GetUserQuizResultAsync(userId, request.QuizId);
            if (existingResult != null)
            {
                throw new InvalidOperationException("Quiz đã được làm trước đó");
            }

            // Validate submission
            var isValid = await _quizRepository.ValidateQuizSubmissionAsync(request, request.QuizId);
            if (!isValid)
            {
                throw new InvalidOperationException("Dữ liệu nộp bài không hợp lệ");
            }

            // Lấy quiz và questions để tính điểm
            var quiz = await _quizRepository.GetQuizWithQuestionsAndOptionsAsync(request.QuizId);
            if (quiz == null)
            {
                throw new InvalidOperationException("Quiz không tồn tại");
            }

            var startTime = DateTime.UtcNow;
            var endTime = DateTime.UtcNow;

            // Tính điểm
            var correctAnswers = 0;
            var questionResults = new List<QuestionResultDTO>();

            foreach (var answer in request.Answers)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);
                if (question == null) continue;

                var selectedOption = question.Options.FirstOrDefault(o => o.OptionId == answer.OptionId);
                var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);

                var isCorrect = selectedOption?.IsCorrect ?? false;
                if (isCorrect) correctAnswers++;

                questionResults.Add(new QuestionResultDTO
                {
                    QuestionId = question.QuestionId,
                    Content = question.Content,
                    SelectedOptionId = answer.OptionId,
                    SelectedOptionContent = selectedOption?.Content ?? "",
                    IsCorrect = isCorrect,
                    CorrectOptionId = correctOption?.OptionId,
                    CorrectOptionContent = correctOption?.Content ?? ""
                });
            }

            var totalQuestions = quiz.Questions.Count;
            var score = totalQuestions > 0 ? (decimal)correctAnswers / totalQuestions * 100 : 0;
            var isPassed = quiz.PassScore == null || score >= quiz.PassScore;

            // Tạo UserQuizResult
            var userQuizResult = new UserQuizResult
            {
                UserId = userId,
                QuizId = request.QuizId,
                Score = score,
                CorrectAnswers = correctAnswers,
                StartTime = startTime,
                EndTime = endTime,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _quizRepository.CreateUserQuizResultAsync(userQuizResult);

            // Nếu pass thì cập nhật tiến độ
            if (isPassed)
            {
                await _courseRepository.UpdateQuizProgressAsync(userId, request.QuizId);
            }

            return new QuizResultResponseDTO
            {
                UserQuizResultId = userQuizResult.UserQuizResultId,
                QuizId = quiz.QuizId,
                QuizName = quiz.QuizName,
                Score = score,
                CorrectAnswers = correctAnswers,
                TotalQuestions = totalQuestions,
                PassScore = quiz.PassScore,
                IsPassed = isPassed,
                StartTime = startTime,
                EndTime = endTime,
                Duration = endTime - startTime,
                QuestionResults = questionResults
            };
        }

        public async Task<QuizResultResponseDTO> GetQuizResultAsync(long quizId, string userId)
        {
            var userQuizResult = await _quizRepository.GetUserQuizResultAsync(userId, quizId);
            if (userQuizResult == null)
            {
                throw new InvalidOperationException("Không tìm thấy kết quả quiz");
            }

            var quiz = await _quizRepository.GetQuizWithQuestionsAndOptionsAsync(quizId);
            if (quiz == null)
            {
                throw new InvalidOperationException("Quiz không tồn tại");
            }

            // Tạo question results (có thể lưu vào database để tối ưu)
            var questionResults = new List<QuestionResultDTO>();
            foreach (var question in quiz.Questions)
            {
                var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                questionResults.Add(new QuestionResultDTO
                {
                    QuestionId = question.QuestionId,
                    Content = question.Content,
                    SelectedOptionId = 0, // Cần lưu UserAnswer để có thông tin này
                    SelectedOptionContent = "",
                    IsCorrect = false, // Cần tính toán lại
                    CorrectOptionId = correctOption?.OptionId,
                    CorrectOptionContent = correctOption?.Content ?? ""
                });
            }

            return new QuizResultResponseDTO
            {
                UserQuizResultId = userQuizResult.UserQuizResultId,
                QuizId = userQuizResult.QuizId,
                QuizName = userQuizResult.Quiz.QuizName,
                Score = userQuizResult.Score,
                CorrectAnswers = userQuizResult.CorrectAnswers,
                TotalQuestions = quiz.TotalQuestions,
                PassScore = quiz.PassScore,
                IsPassed = quiz.PassScore == null || userQuizResult.Score >= quiz.PassScore,
                StartTime = userQuizResult.StartTime,
                EndTime = userQuizResult.EndTime,
                Duration = userQuizResult.EndTime - userQuizResult.StartTime,
                QuestionResults = questionResults
            };
        }

        public async Task<IEnumerable<QuizResultResponseDTO>> GetUserQuizResultsAsync(string userId)
        {
            var userQuizResults = await _quizRepository.GetUserQuizResultsAsync(userId);
            var results = new List<QuizResultResponseDTO>();

            foreach (var result in userQuizResults)
            {
                var quiz = await _quizRepository.GetQuizByIdAsync(result.QuizId);
                if (quiz == null) continue;

                results.Add(new QuizResultResponseDTO
                {
                    UserQuizResultId = result.UserQuizResultId,
                    QuizId = result.QuizId,
                    QuizName = result.Quiz.QuizName,
                    Score = result.Score,
                    CorrectAnswers = result.CorrectAnswers,
                    TotalQuestions = quiz.TotalQuestions,
                    PassScore = quiz.PassScore,
                    IsPassed = quiz.PassScore == null || result.Score >= quiz.PassScore,
                    StartTime = result.StartTime,
                    EndTime = result.EndTime,
                    Duration = result.EndTime - result.StartTime,
                    QuestionResults = new List<QuestionResultDTO>() // Có thể load chi tiết nếu cần
                });
            }

            return results;
        }

        public async Task<bool> IsQuizCompletedAsync(long quizId, string userId)
        {
            return await _quizRepository.IsQuizCompletedAsync(userId, quizId);
        }

        public async Task<bool> UpdateQuizProgressAsync(string userId, long quizId)
        {
            try
            {
                // Lấy thông tin quiz để biết module
                var quiz = await _quizRepository.GetQuizByIdAsync(quizId);
                if (quiz == null) return false;

                // Cập nhật tiến độ module thông qua CourseService
                return await _courseRepository.UpdateQuizProgressAsync(userId, quizId);
            }
            catch
            {
                return false;
            }
        }
    }
} 