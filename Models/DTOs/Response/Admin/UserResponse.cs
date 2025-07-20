using Online_Learning.Constants.Enums;

namespace Online_Learning.Models.DTOs.Response.Admin
{
    public class UserResponse
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateOnly? DoB { get; set; }
        public bool? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<UserRole> Roles { get; set; } = new List<UserRole>();
        public int TotalCourses { get; set; }
        public int CompletedCourses { get; set; }
    }
}
