namespace Online_Learning.Models.DTOs.Response.Admin.OptionDto
{
    public class OptionResponseDto
    {
        public long OptionID { get; set; }
        public long QuestionID { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Status { get; set; }
    }
}
