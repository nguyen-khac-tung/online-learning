namespace Online_Learning.Models.DTOs.Response.Admin.ModuleDto
{
    public class ModuleResponseDto
    {
        public long ModuleID { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public int ModuleNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Status { get; set; }
        public string CourseID { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int LessonCount { get; set; }
        public int QuizCount { get; set; }
    }
}
