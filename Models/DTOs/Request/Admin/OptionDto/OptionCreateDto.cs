using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.OptionDto
{
    public class OptionCreateDto
    {
        [Required(ErrorMessage = "Nội dung tùy chọn là bắt buộc")]
        [StringLength(255, ErrorMessage = "Nội dung tùy chọn tối đa 255 ký tự")]
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;

        [Range(0, 1, ErrorMessage = "Trạng thái phải là 0 (Bản nháp) hoặc 1 (Hoạt động)")]
        public int Status { get; set; } = 1;
    }
}
