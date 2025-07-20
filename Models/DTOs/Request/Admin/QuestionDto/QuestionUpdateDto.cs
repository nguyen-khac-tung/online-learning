using Online_Learning.Models.DTOs.Request.Admin.OptionDto;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.QuestionDto
{
    public class QuestionUpdateDto
    {
        [Required(ErrorMessage = "ID câu hỏi là bắt buộc")]
        public long QuestionID { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Số thứ tự câu hỏi phải lớn hơn hoặc bằng 0")]
        public int QuestionNum { get; set; }
        [Required(ErrorMessage = "Nội dung câu hỏi là bắt buộc")]
        [StringLength(255, ErrorMessage = "Nội dung câu hỏi tối đa 255 ký tự")]
        public string Content { get; set; } = string.Empty;
        [Range(0, 1, ErrorMessage = "Loại câu hỏi phải là 0 (Trắc nghiệm) hoặc 1 (Tự luận)")]
        public int Type { get; set; } = 0;
        [Range(0, 1, ErrorMessage = "Trạng thái phải là 0 (Bản nháp) hoặc 1 (Hoạt động)")]
        public int Status { get; set; } = 1;
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 tùy chọn trả lời")]
        public List<OptionUpdateDto> Options { get; set; } = new List<OptionUpdateDto>();
    }
}
