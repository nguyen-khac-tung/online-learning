using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin.CoursePrice
{
    public class CoursePriceCreateDto
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; }
    }
}
