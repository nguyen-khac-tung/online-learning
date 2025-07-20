using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.User
{
    public class CartRequestDto
    {
        [Required(ErrorMessage = "CourseID can not be empty")]
        public string CourseId { get; set; }
    }
}
