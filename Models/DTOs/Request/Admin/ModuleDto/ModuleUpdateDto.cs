using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.ModuleDto
{
    public class ModuleUpdateDto
    {
        [Required]
        [StringLength(255)]
        public string ModuleName { get; set; } = string.Empty;

        public int ModuleNumber { get; set; }

        public int Status { get; set; }
    }
}
