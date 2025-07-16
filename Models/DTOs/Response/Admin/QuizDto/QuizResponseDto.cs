using Online_Learning.Models.DTOs.Response.Admin.QuestionDto;

namespace Online_Learning.Models.DTOs.Response.Admin.QuizDto
{
    public class QuizResponseDto
    {
        public long QuizID { get; set; }
        public long ModuleID { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string QuizName { get; set; } = string.Empty;
        public int? QuizTime { get; set; }
        public int TotalQuestions { get; set; }
        public int? PassScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Status { get; set; }
        public List<QuestionResponseDto> Questions { get; set; } = new List<QuestionResponseDto>();
    }
}
