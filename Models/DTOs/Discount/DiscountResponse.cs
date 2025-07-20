namespace Online_Learning.Models.DTOs.Discount
{
    public class DiscountResponse
    {
        public long DiscountId { get; set; }
        public string Code { get; set; } = null!;
        public decimal? FixValue { get; set; }
        public decimal? MaxValue { get; set; }
        public double? PercentageValue { get; set; }
        public decimal? MinPurchase { get; set; }
        public string Creator { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!;
        public int Used { get; set; }
        public int Quantity { get; set; }
        public int MaxUse { get; set; }
        public int Remaining { get; set; }
        public string? Description { get; set; }
    }
}
