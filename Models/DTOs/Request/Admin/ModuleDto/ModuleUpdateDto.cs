using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.ModuleDto
{
    public class ModuleUpdateDto
    {
        [Required(ErrorMessage = "Tên module là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên module tối đa 255 ký tự")]
        public string ModuleName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Số thứ tự module phải lớn hơn hoặc bằng 0")]
        public int ModuleNumber { get; set; }

        public int Status { get; set; }
    }
}
