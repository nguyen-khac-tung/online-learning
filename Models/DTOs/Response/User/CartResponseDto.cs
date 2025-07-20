using Online_Learning.Constants.Enums;
using Online_Learning.Models.Entities;

namespace Online_Learning.Models.DTOs.Response.User
{
    public class CartItemsDto
    {
        public long CartItemId { get; set; }

        public string CourseId { get; set; } = null!;

        public string CourseName { get; set; } = null!;

        public string? CourseImgUrl { get; set; } = null!;

        public string StudyTime { get; set; } = null!;

        public List<string> Category { get; set; }

        public string LevelName { get; set; }

        public string Language { get; set; }

        public decimal Price { get; set; }

    }
}
