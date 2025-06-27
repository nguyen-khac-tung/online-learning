using Online_Learning.Constants.Enums;
using Online_Learning.Models.Entities;

namespace Online_Learning.Models.DTOs.Response.User
{
	// DTO cho Quiz
	public class QuizResponseDTO
	{
		public long QuizId { get; set; }
		public string QuizName { get; set; } = null!;
		public int? QuizTime { get; set; }
		public int TotalQuestions { get; set; }
		public int? PassScore { get; set; }
		public QuizStatus Status { get; set; }
		public string StatusName => Status.ToString();

		public QuizResponseDTO(Quiz quiz)
		{
			QuizId = quiz.QuizId;
			QuizName = quiz.QuizName;
			QuizTime = quiz.QuizTime;
			TotalQuestions = quiz.TotalQuestions;
			PassScore = quiz.PassScore;
			Status = (QuizStatus)quiz.Status;
		}
	}
}
