using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.ModuleDto
{
    public class ModuleCreateDto
    {
        [Required(ErrorMessage = "Tên module là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên module tối đa 255 ký tự")]
        public string ModuleName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Số thứ tự module phải lớn hơn hoặc bằng 0")]
        public int ModuleNumber { get; set; }

        [Required(ErrorMessage = "Khóa học là bắt buộc")]
        [StringLength(36, ErrorMessage = "ID khóa học tối đa 36 ký tự")]
        public string CourseID { get; set; } = string.Empty;

        public int status { get; set; } 
    }
}
