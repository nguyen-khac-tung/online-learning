namespace Online_Learning.Models.DTOs.Rating
{
    public class CreateRatingRequest
    {
        public string UserId { get; set; }
        public string CourseID { get; set; } = null!;
        public byte Score { get; set; }
        public string? ReviewText { get; set; }
    }
}
