using Online_Learning.Constants;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.ModuleDto
{
    public class ModuleCreateDto
    {
        [Required(ErrorMessage = Messages.Required)]
        [StringLength(255, ErrorMessage = Messages.MaxLength255)]
        public string ModuleName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = Messages.RangeGreaterOrEqual0)]
        public int ModuleNumber { get; set; }

        [Required(ErrorMessage = Messages.Required)]
        [StringLength(36, ErrorMessage = "ID khóa học tối đa 36 ký tự")]
        public string CourseID { get; set; } = string.Empty;

        public int status { get; set; } 
    }
}
