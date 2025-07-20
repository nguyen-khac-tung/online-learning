using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Comment
{
    public class CreateCommentRequest
    {
        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; } = null!;

        public long? ParentCommentId { get; set; }

        [Required(ErrorMessage = "LessonId is required")]
        public long LessonId { get; set; }
    }
}
