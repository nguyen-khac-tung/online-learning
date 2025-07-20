namespace Online_Learning.Models.DTOs.Comment
{
    public class CommentFilterRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public long? LessonId { get; set; }
        public string? UserId { get; set; }
        public int? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; } = true;
    }
}
