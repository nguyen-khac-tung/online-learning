namespace Online_Learning.Models.DTOs.Response.Admin.CoursePrice
{
    public class CoursePriceResponseDto
    {
        public long CoursePriceID { get; set; }
        public string CourseID { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
