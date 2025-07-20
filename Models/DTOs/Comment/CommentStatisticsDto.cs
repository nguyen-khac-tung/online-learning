namespace Online_Learning.Models.DTOs.Comment
{
    public class CommentStatisticsDto
    {
        public int TotalComments { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int ReportedCount { get; set; }
        public string MostActiveUser { get; set; } = string.Empty;
        public Dictionary<string, int> CommentsByMonth { get; set; } = new();
        public Dictionary<string, int> CommentsByStatus { get; set; } = new();

    }
}
