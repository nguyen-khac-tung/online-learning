using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Auth
{
    public class UserLogin {
        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password cannot be empty")]
        public string Password { get; set; }
    }

    public class UserRegisterRequest
    {
        [Required(ErrorMessage = "Full Name cannot be empty")]
        [MaxLength(100, ErrorMessage = "Full Name cannot be over 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password cannot be empty")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
    }

    public class VerifyRegisterRequest
    {
        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP cannot be empty")]
        public string OtpCode { get; set; }

        [Required(ErrorMessage = "Full Name cannot be empty")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Password cannot be empty")]
        public string Password { get; set; }
    }

    public class SendOtpRequest
    {
        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; }
    }

    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; }
    }

    public class VerifyForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP cannot be empty")]
        public string OtpCode { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "New Password cannot be empty")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Comfirm New Password cannot be empty")]
        [Compare("NewPassword", ErrorMessage = "Confirm New Password must match the New Password")]
        public string ConfirmNewPassword { get; set; }
    }
}
