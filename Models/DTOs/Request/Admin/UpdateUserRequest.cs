using Online_Learning.Constants;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = Messages.FullNameCannotBeEmpty)]
        [MaxLength(100, ErrorMessage = Messages.MaxLength100)]
        public string FullName { get; set; } = null!;

        public DateOnly? DoB { get; set; }
        public bool? Gender { get; set; }

        [Phone(ErrorMessage = Messages.InvalidPhone)]
        public string? Phone { get; set; }

        [MaxLength(255, ErrorMessage = Messages.AddressMaxLength)]
        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
