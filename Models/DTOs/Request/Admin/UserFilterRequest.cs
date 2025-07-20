using Online_Learning.Constants.Enums;

namespace Online_Learning.Models.DTOs.Request.Admin
{
    public class UserFilterRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public UserRole? Role { get; set; }
        public UserStatus? Status { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; } = true;
    }
}
