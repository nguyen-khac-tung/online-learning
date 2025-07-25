using Online_Learning.Constants;
using Online_Learning.Models.DTOs.Request.Admin.OptionDto;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.QuestionDto
{
    public class QuestionCreateDto
    {
        [Required(ErrorMessage = Messages.Required)]
        public long QuizID { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = Messages.RangeGreaterOrEqual0)]
        public int QuestionNum { get; set; }

        [Required(ErrorMessage = Messages.Required)]
        [StringLength(255, ErrorMessage = Messages.MaxLength255)]
        public string Content { get; set; } = string.Empty;

        [Range(0, 1, ErrorMessage = Messages.Range0Or1)]
        public int Type { get; set; } = 0;

        [Range(0, 1, ErrorMessage = Messages.Range0Or1)]
        public int Status { get; set; } = 1;

        [MinLength(1, ErrorMessage = Messages.MinLength1)]
        public List<OptionCreateDto> Options { get; set; } = new List<OptionCreateDto>();
    }
}
