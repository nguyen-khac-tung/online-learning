using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.User
{
    public class UpdateProfileRequestDto
    {
        [Required(ErrorMessage = "Full Name cannot be empty")]
        [MaxLength(100, ErrorMessage = "Full Name cannot be over 100 characters")]
        public string FullName { get; set; }

        public DateOnly? DoB { get; set; }

        public bool? Gender { get; set; }

        [RegularExpression("^\\d{10,11}$", ErrorMessage = "Phone number is not valid")]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
