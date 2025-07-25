using Online_Learning.Constants;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.ModuleDto
{
    public class ModuleUpdateDto
    {
        [Required(ErrorMessage = Messages.Required)]
        [StringLength(255, ErrorMessage = Messages.MaxLength255)]
        public string ModuleName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = Messages.RangeGreaterOrEqual0)]
        public int ModuleNumber { get; set; }

        public int Status { get; set; }
    }
}
