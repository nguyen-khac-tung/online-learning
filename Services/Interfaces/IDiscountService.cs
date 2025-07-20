using Online_Learning.Models.DTOs.Discount;

namespace Online_Learning.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<DiscountResponse> CreateAsync(DiscountRequest request);
        Task<PagedResult<DiscountResponse>> GetValidDiscountsAsync(DateTime? date, int? status, int page = 1, int pageSize = 10, string? search = null);
        Task<DiscountUsageInfo> GetUsageInfoAsync(long id);
        Task<DiscountResponse> UpdateAsync(long id, DiscountUpdateRequest request);
        Task SoftDeleteAsync(long id);
        Task<DiscountResponse> ToggleStatusAsync(long id);
    }
}