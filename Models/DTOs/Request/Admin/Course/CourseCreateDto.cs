using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Online_Learning.Models.DTOs.Request.Admin.Course
{
    public class CourseCreateDto
    {
        [Required]
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public string Creator { get; set; } = string.Empty;
        [Required]
        public string StudyTime { get; set; } = string.Empty;
        public int? LevelID { get; set; }
        public int? LanguageID { get; set; }
        // public int CertificateID { get; set; } // Removed
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal? Price { get; set; }
        public List<IFormFile>? AttachmentFiles { get; set; }
        public List<int> CategoryIDs { get; set; } = new List<int>();
    }
}
