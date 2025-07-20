using Online_Learning.Models.DTOs.Response.Admin.OptionDto;

namespace Online_Learning.Models.DTOs.Response.Admin.QuestionDto
{
    public class QuestionResponseDto
    {
        public long QuestionID { get; set; }
        public long QuizID { get; set; }
        public int QuestionNum { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Status { get; set; }
        public List<OptionResponseDto> Options { get; set; } = new List<OptionResponseDto>();
    }
}
