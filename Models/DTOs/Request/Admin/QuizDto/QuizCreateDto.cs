using Online_Learning.Models.DTOs.Request.Admin.QuestionDto;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.QuizDto
{
    public class QuizCreateDto
    {
        [Required]
        public long ModuleID { get; set; }

        [Required]
        [StringLength(255)]
        public string QuizName { get; set; } = string.Empty;

        public int? QuizTime { get; set; }

        public int? PassScore { get; set; }

        public List<QuestionCreateDto> Questions { get; set; } = new List<QuestionCreateDto>();
    }
}
