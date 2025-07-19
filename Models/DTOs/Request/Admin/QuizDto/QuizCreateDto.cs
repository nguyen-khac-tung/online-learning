using Online_Learning.Models.DTOs.Request.Admin.QuestionDto;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.QuizDto
{
    public class QuizCreateDto
    {
        [Required]
        public long ModuleID { get; set; }

        [Required(ErrorMessage = "Tên câu đố là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên câu đố tối đa 255 ký tự")]
        public string QuizName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Thời gian làm bài phải lớn hơn 0")]
        public int? QuizTime { get; set; }

        [Range(0, 100, ErrorMessage = "Điểm đạt phải từ 0 đến 100")]
        public int? PassScore { get; set; }

        public List<QuestionCreateDto> Questions { get; set; } = new List<QuestionCreateDto>();
    }
}
