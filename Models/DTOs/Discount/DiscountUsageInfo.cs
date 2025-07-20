namespace Online_Learning.Models.DTOs.Discount
{
    public class DiscountUsageInfo
    {
        public string Code { get; set; } = null!;
        public int Used { get; set; }
        public int Remaining { get; set; }
        public string Status { get; set; } = null!;
    }
}
