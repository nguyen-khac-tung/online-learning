namespace Online_Learning.Models.DTOs.Response.User
{

    public class UserDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class UserProfileDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateOnly? DoB { get; set; }
        public bool? Gender { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string AvatarUrl { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
