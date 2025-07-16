namespace Online_Learning.Models.DTOs.Response.Admin.LessonDto
{
    public class LessonResponseDto
    {
        public long LessonID { get; set; }
        public long ModuleID { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public int LessonNumber { get; set; }
        public string LessonName { get; set; } = string.Empty;
        public string? LessonContent { get; set; }
        public string? LessonVideo { get; set; }
        public int? Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Status { get; set; }
    }
}
