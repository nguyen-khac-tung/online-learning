using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.ModuleDto
{
    public class ModuleCreateDto
    {
        [Required]
        [StringLength(255)]
        public string ModuleName { get; set; } = string.Empty;

        public int ModuleNumber { get; set; }

        [Required]
        public string CourseID { get; set; } = string.Empty;

        public int status { get; set; } 
    }
}
