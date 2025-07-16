using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.QuizDto
{
    public class QuizUpdateDto
    {
        [Required]
        [StringLength(255)]
        public string QuizName { get; set; } = string.Empty;

        public int? QuizTime { get; set; }

        public int? PassScore { get; set; }

        public int Status { get; set; }
    }
}
