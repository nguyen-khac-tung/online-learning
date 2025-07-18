using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.LessonDto
{
    public class LessonUpdateDto
    {
        public int LessonNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string LessonName { get; set; } = string.Empty;

        public string? LessonContent { get; set; }

        public string? LessonVideo { get; set; }

        public int? Duration { get; set; }

        public int? Status { get; set; }
    }
}
