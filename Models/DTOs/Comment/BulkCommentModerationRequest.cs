using System.ComponentModel.DataAnnotations;


namespace Online_Learning.Models.DTOs.Comment
{
    public class BulkCommentModerationRequest
    {
        [Required]
        public List<long> CommentIds { get; set; } = new();

        [Required]
        [Range(1, 2, ErrorMessage = "Action must be 1 (Approve) or 2 (Reject)")]
        public int Action { get; set; }
    }
}
