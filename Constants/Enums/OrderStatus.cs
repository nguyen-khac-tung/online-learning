namespace Online_Learning.Constants.Enums
{
    public enum OrderStatus
    {
        Pending = 0,         // Chờ xử lý
        Confirmed = 1,       // Đã xác nhận
        Processing = 2,      // Đang xử lý
        Completed = 3,       // Hoàn thành
        Cancelled = 4,       // Đã hủy
        Refunded = 5,        // Đã hoàn tiền
        Failed = 6           // Thất bại
    }
}