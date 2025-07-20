using Online_Learning.Constants.Enums;
using Online_Learning.Models.Entities;

namespace Online_Learning.Models.DTOs.Response.User
{
	// DTO cho Quiz
	public class QuizResponseDTO
	{
		public long QuizId { get; set; }
		public string QuizName { get; set; } = string.Empty;
		public int? QuizTime { get; set; }
		public int TotalQuestions { get; set; }
		public int? PassScore { get; set; }
		public List<QuestionResponseDTO> Questions { get; set; } = new List<QuestionResponseDTO>();
	}

	public class QuestionResponseDTO
	{
		public long QuestionId { get; set; }
		public int QuestionNum { get; set; }
		public string Content { get; set; } = string.Empty;
		public int Type { get; set; }
		public List<OptionResponseDTO> Options { get; set; } = new List<OptionResponseDTO>();
	}

	public class OptionResponseDTO
	{
		public long OptionId { get; set; }
		public string Content { get; set; } = string.Empty;
	}
}
