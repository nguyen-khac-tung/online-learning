using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Comment
{
    public class CommentModerationRequest
    {
        [Required]
        public long CommentId { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Action must be 1 (Approve) or 2 (Reject)")]
        public int Action { get; set; }

        public string? Reason { get; set; }
    }
}
