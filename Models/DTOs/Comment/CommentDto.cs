namespace Online_Learning.Models.DTOs.Comment
{
    public class CommentDto
    {
        public long CommentId { get; set; }
        public long? ParentCommentId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserId { get; set; } = null!;
        public long? LessonId { get; set; }
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string? UserAvatarUrl { get; set; }
        public string? LessonName { get; set; }
        public string? ModuleName { get; set; }
        public string? CourseName { get; set; }
        public int? Status { get; set; }
        public string StatusText { get; set; } = null!;
        public int ReplyCount { get; set; }
        public bool IsReported { get; set; }
        public int ReportCount { get; set; }
        
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
    }
}
