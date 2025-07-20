namespace Online_Learning.Models.DTOs.Rating
{
    public class RatingResponse
    {
        public long RatingID { get; set; }
        public string CourseID { get; set; } = null!;
        public string UserID { get; set; } = null!;
        public byte Score { get; set; }
        public string? ReviewText { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
