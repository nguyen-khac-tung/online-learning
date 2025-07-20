using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Online_Learning.Models.DTOs.Request.Admin.Course
{
    public class CourseCreateDto
    {
        [Required(ErrorMessage = "Tên khóa học là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khóa học tối đa 100 ký tự")]
        public string CourseName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Người tạo là bắt buộc")]
        public string Creator { get; set; } = string.Empty;
        [Required(ErrorMessage = "Thời gian học là bắt buộc")]
        public string StudyTime { get; set; } = string.Empty;
        [Required(ErrorMessage = "Cấp độ là bắt buộc")]
        public int? LevelID { get; set; }
        [Required(ErrorMessage = "Ngôn ngữ là bắt buộc")]
        public int? LanguageID { get; set; }

        public int Status { get; set; } 
        // public int CertificateID { get; set; } // Removed
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal? Price { get; set; }
        public List<IFormFile>? AttachmentFiles { get; set; }
        [MinLength(1, ErrorMessage = "Phải chọn ít nhất 1 danh mục")]
        public List<int> CategoryIDs { get; set; } = new List<int>();
    }
}
