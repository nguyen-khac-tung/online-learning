using Online_Learning.Constants.Enums;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; } = null!;

        public DateOnly? DoB { get; set; }
        public bool? Gender { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [MaxLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public List<UserRole> Roles { get; set; } = new List<UserRole>();
    }
}
