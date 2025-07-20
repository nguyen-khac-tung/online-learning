namespace Online_Learning.Models.DTOs.Response.User;

public class QuizResultResponseDTO
{
    public long UserQuizResultId { get; set; }
    public long QuizId { get; set; }
    public string QuizName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public int? PassScore { get; set; }
    public bool IsPassed { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public List<QuestionResultDTO> QuestionResults { get; set; } = new List<QuestionResultDTO>();
}

public class QuestionResultDTO
{
    public long QuestionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public long SelectedOptionId { get; set; }
    public string SelectedOptionContent { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public long? CorrectOptionId { get; set; }
    public string? CorrectOptionContent { get; set; } = string.Empty;
} 