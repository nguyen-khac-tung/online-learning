
namespace Online_Learning.Models.DTOs.Discount
{
    public class DiscountRequest
    {
        public string Code { get; set; } = null!;
        public decimal? FixValue { get; set; }
        public decimal? MaxValue { get; set; }
        public double? PercentageValue { get; set; }
        public decimal? MinPurchase { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MaxUse { get; set; }
        public int? Quantity { get; set; }
        public string Creator { get; set; } = null!;
    }
}
