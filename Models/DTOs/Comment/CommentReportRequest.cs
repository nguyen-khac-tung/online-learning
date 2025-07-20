using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Comment
{
    public class CommentReportRequest
    {
        [Required(ErrorMessage = "CommentId is required")]
        public long CommentId { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string Reason { get; set; } = null!;

        public int ReportType { get; set; } // 1: Spam, 2: Inappropriate, 3: Offensive, 4: Other
    }
}
