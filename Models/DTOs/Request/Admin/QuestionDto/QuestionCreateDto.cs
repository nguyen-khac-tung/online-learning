using Online_Learning.Models.DTOs.Request.Admin.OptionDto;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.QuestionDto
{
    public class QuestionCreateDto
    {
        public long QuizID { get; set; }
        public int QuestionNum { get; set; }
        [Required]
        [StringLength(255)]
        public string Content { get; set; } = string.Empty;
        public int Type { get; set; } = 0;
        public int Status { get; set; } = 1;
        public List<OptionCreateDto> Options { get; set; } = new List<OptionCreateDto>();
    }
}
