using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.OptionDto
{
    public class OptionCreateDto
    {
        [Required]
        [StringLength(255)]
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
        public int Status { get; set; } = 1;
    }
}
