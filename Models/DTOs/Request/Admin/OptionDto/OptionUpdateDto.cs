using Online_Learning.Constants;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.OptionDto
{
    public class OptionUpdateDto
    {
        [Required(ErrorMessage = Messages.Required)]
        public long OptionID { get; set; }
        [Required(ErrorMessage = Messages.Required)]
        [StringLength(255, ErrorMessage = Messages.MaxLength255)]
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
        [Range(0, 1, ErrorMessage = Messages.Range0Or1)]
        public int Status { get; set; } = 1;
    }
}
