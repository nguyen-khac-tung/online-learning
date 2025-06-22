namespace Online_Learning.Constants.Enums
{
    public enum UserStatus
    {
        Active = 1,          // Hoạt động
        Inactive = 0,        // Không hoạt động
        Banned = 2,          // Bị cấm
        Pending = 3,         // Chờ xác thực
        Deleted = -1         // Đã xóa
    }
}