namespace Online_Learning.Models.DTOs.Response.Admin
{
    public class UserStatisticsResponse
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int BannedUsers { get; set; }
        public int PendingUsers { get; set; }
        public int AdminUsers { get; set; }
        public int StudentUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int TotalCoursesEnrolled { get; set; }
        public int TotalCoursesCompleted { get; set; }
    }
}
