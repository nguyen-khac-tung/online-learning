using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.LessonDto
{
    public class LessonCreateDto
    {
        [Required]
        public long ModuleID { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số thứ tự bài học phải lớn hơn hoặc bằng 0")]
        public int LessonNumber { get; set; }

        [Required(ErrorMessage = "Tên bài học là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên bài học tối đa 255 ký tự")]
        public string LessonName { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Nội dung bài học tối đa 2000 ký tự")]
        public string? LessonContent { get; set; }

        [StringLength(500, ErrorMessage = "Link video tối đa 500 ký tự")]
        public string? LessonVideo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Thời lượng phải lớn hơn hoặc bằng 0")]
        public int? Duration { get; set; }

        public int Status { get; set; } 
    }
}
