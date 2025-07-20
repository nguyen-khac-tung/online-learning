using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin
{
    public class UpdateUserRequest
    {
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
    }
}
