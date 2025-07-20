using Online_Learning.Models.DTOs.Discount;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Interfaces;
using Online_Learning.Constants.Enums;

namespace Online_Learning.Services.Implementations
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _repository;

        public DiscountService(IDiscountRepository repository)
        {
            _repository = repository;
        }

        public async Task<DiscountResponse> CreateAsync(DiscountRequest request)
        {
            // Validate request
            ValidateDiscountRequest(request);

            // Check if discount code already exists
            var existingDiscount = await _repository.GetByCodeAsync(request.Code);
            if (existingDiscount != null && existingDiscount.Status != (int)DiscountStatus.Deleted)
            {
                throw new Exception("Discount code already exists");
            }

            var discount = new Discount
            {
                Code = request.Code.Trim().ToUpper(),
                FixValue = request.FixValue,
                PercentageValue = request.PercentageValue,
                MaxValue = request.MaxValue,
                MinPurchase = request.MinPurchase,
                StartDate = DateOnly.FromDateTime(request.StartDate),
                EndDate = DateOnly.FromDateTime(request.EndDate),
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Creator = request.Creator,
                Quantity = request.Quantity ?? 100,
                MaxUse = request.MaxUse ?? 1,
                Used = 0,
                Status = (int)DiscountStatus.Active
            };

            var result = await _repository.AddAsync(discount);
            return Map(result);
        }

        public async Task<PagedResult<DiscountResponse>> GetValidDiscountsAsync(DateTime? date, int? status, int page = 1, int pageSize = 10, string? search = null)
        {
            var all = await _repository.GetAllAsync();
            var today = DateOnly.FromDateTime(date ?? DateTime.Now);
            var filteredDiscounts = new List<Discount>();

            foreach (var d in all)
            {
                if (d.Status == (int)DiscountStatus.Deleted) continue;

                var currentStatus = GetCurrentStatus(d);
                if (d.Status != currentStatus)
                {
                    d.Status = currentStatus;
                    await _repository.UpdateAsync(d);
                }

                bool matchesStatus = !status.HasValue || d.Status == status;

                bool matchesSearch = string.IsNullOrEmpty(search) ||
                                     d.Code.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                     (d.Creator?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false);

                if (matchesStatus && matchesSearch)
                {
                    filteredDiscounts.Add(d);
                }
            }

            var totalCount = filteredDiscounts.Count;
            var items = filteredDiscounts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(Map)
                .ToList();

            return new PagedResult<DiscountResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        public async Task<DiscountUsageInfo> GetUsageInfoAsync(long id)
        {
            var discount = await _repository.GetByIdAsync(id);
            if (discount == null || discount.Status == (int)DiscountStatus.Deleted)
                throw new Exception("Discount not found");

            var currentStatus = GetCurrentStatus(discount);
            if (discount.Status != currentStatus)
            {
                discount.Status = currentStatus;
                await _repository.UpdateAsync(discount);
            }

            return new DiscountUsageInfo
            {
                Code = discount.Code,
                Used = discount.Used ?? 0,
                Remaining = (discount.Quantity ?? 0) - (discount.Used ?? 0),
                Status = GetStatusName(discount.Status ?? 1)
            };
        }

        public async Task<DiscountResponse> UpdateAsync(long id, DiscountUpdateRequest request)
        {
            var discount = await _repository.GetByIdAsync(id);
            if (discount == null || discount.Status == (int)DiscountStatus.Deleted)
                throw new Exception("Discount not found");

            // Validate update request
            ValidateDiscountUpdateRequest(request);

            // Check if new code already exists (if code is being changed)
            if (!string.IsNullOrEmpty(request.Code) &&
                !discount.Code.Equals(request.Code.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                var existingDiscount = await _repository.GetByCodeAsync(request.Code.Trim());
                if (existingDiscount != null && existingDiscount.DiscountId != id &&
                    existingDiscount.Status != (int)DiscountStatus.Deleted)
                {
                    throw new Exception("Discount code already exists");
                }
                discount.Code = request.Code.Trim().ToUpper();
            }

            // Update fields if provided
            if (request.FixValue.HasValue) discount.FixValue = request.FixValue;
            if (request.PercentageValue.HasValue) discount.PercentageValue = request.PercentageValue;
            if (request.MaxValue.HasValue) discount.MaxValue = request.MaxValue;
            if (request.MinPurchase.HasValue) discount.MinPurchase = request.MinPurchase;
            if (request.StartDate.HasValue) discount.StartDate = DateOnly.FromDateTime(request.StartDate.Value);
            if (request.EndDate.HasValue) discount.EndDate = DateOnly.FromDateTime(request.EndDate.Value);
            if (request.Quantity.HasValue) discount.Quantity = request.Quantity;
            if (request.MaxUse.HasValue) discount.MaxUse = request.MaxUse;
            if (!string.IsNullOrEmpty(request.Description)) discount.Description = request.Description;

            // Sử dụng CreatedAt để lưu thông tin cập nhật cuối cùng
            // Format: "Created: 2024-01-01 10:00:00 | Updated: 2024-01-02 15:30:00"
            var createdTime = ExtractCreatedTime(discount.CreatedAt);
            discount.CreatedAt = $"{createdTime} | Updated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            var result = await _repository.UpdateAsync(discount);
            return Map(result);
        }

        public async Task SoftDeleteAsync(long id)
        {
            var discount = await _repository.GetByIdAsync(id);
            if (discount == null || discount.Status == (int)DiscountStatus.Deleted)
                throw new Exception("Discount not found");

            discount.Status = (int)DiscountStatus.Deleted;

            // Cập nhật thông tin xóa vào CreatedAt
            var createdTime = ExtractCreatedTime(discount.CreatedAt);
            discount.CreatedAt = $"{createdTime} | Deleted: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            await _repository.UpdateAsync(discount);
        }

        public async Task<DiscountResponse> ToggleStatusAsync(long id)
        {
            var discount = await _repository.GetByIdAsync(id);
            if (discount == null || discount.Status == (int)DiscountStatus.Deleted)
                throw new Exception("Discount not found");

            // Only allow toggling between Active and Inactive
            if (discount.Status == (int)DiscountStatus.Active)
            {
                discount.Status = (int)DiscountStatus.Inactive;
            }
            else if (discount.Status == (int)DiscountStatus.Inactive)
            {
                discount.Status = (int)DiscountStatus.Active;
            }
            else
            {
                throw new Exception("Cannot change status of expired or used up discount");
            }

            // Cập nhật thông tin thay đổi status
            var createdTime = ExtractCreatedTime(discount.CreatedAt);
            var statusName = discount.Status == (int)DiscountStatus.Active ? "Activated" : "Deactivated";
            discount.CreatedAt = $"{createdTime} | {statusName}: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            var result = await _repository.UpdateAsync(discount);
            return Map(result);
        }

        private int GetCurrentStatus(Discount d)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            // Don't change deleted or inactive status automatically
            if (d.Status == (int)DiscountStatus.Deleted || d.Status == (int)DiscountStatus.Inactive)
                return d.Status ?? (int)DiscountStatus.Active;

            if (d.EndDate.HasValue && d.EndDate < today)
                return (int)DiscountStatus.Expired;

            if ((d.Quantity ?? 0) <= (d.Used ?? 0))
                return (int)DiscountStatus.UsedUp;

            return (int)DiscountStatus.Active;
        }

        private void ValidateDiscountRequest(DiscountRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new Exception("Discount code is required");

            if (request.Code.Length < 3 || request.Code.Length > 20)
                throw new Exception("Discount code must be between 3-20 characters");

            if (!request.FixValue.HasValue && !request.PercentageValue.HasValue)
                throw new Exception("Either fix value or percentage value must be provided");

            if (request.FixValue.HasValue && request.FixValue <= 0)
                throw new Exception("Fix value must be greater than 0");

            if (request.PercentageValue.HasValue && (request.PercentageValue <= 0 || request.PercentageValue > 100))
                throw new Exception("Percentage value must be between 0-100");

            if (request.StartDate >= request.EndDate)
                throw new Exception("Start date must be before end date");

            if (request.Quantity.HasValue && request.Quantity <= 0)
                throw new Exception("Quantity must be greater than 0");

            if (request.MaxUse.HasValue && request.MaxUse <= 0)
                throw new Exception("Max use must be greater than 0");
        }

        private void ValidateDiscountUpdateRequest(DiscountUpdateRequest request)
        {
            if (!string.IsNullOrEmpty(request.Code) && (request.Code.Length < 3 || request.Code.Length > 20))
                throw new Exception("Discount code must be between 3-20 characters");

            if (request.FixValue.HasValue && request.FixValue <= 0)
                throw new Exception("Fix value must be greater than 0");

            if (request.PercentageValue.HasValue && (request.PercentageValue <= 0 || request.PercentageValue > 100))
                throw new Exception("Percentage value must be between 0-100");

            if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate >= request.EndDate)
                throw new Exception("Start date must be before end date");

            if (request.Quantity.HasValue && request.Quantity <= 0)
                throw new Exception("Quantity must be greater than 0");

            if (request.MaxUse.HasValue && request.MaxUse <= 0)
                throw new Exception("Max use must be greater than 0");
        }

        private string ExtractCreatedTime(string? createdAt)
        {
            if (string.IsNullOrEmpty(createdAt))
                return $"Created: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            // Nếu đã có format với "Created:", chỉ lấy phần created
            if (createdAt.Contains("Created:"))
            {
                var parts = createdAt.Split('|');
                return parts[0].Trim();
            }

            // Nếu chỉ có timestamp thuần, thêm prefix "Created:"
            return $"Created: {createdAt}";
        }
        private string GetStatusName(int status) => status switch
        {
            (int)DiscountStatus.Active => "Active",
            (int)DiscountStatus.Inactive => "Inactive",
            (int)DiscountStatus.Expired => "Expired",
            (int)DiscountStatus.UsedUp => "Used Up",
            (int)DiscountStatus.Deleted => "Deleted",
            _ => "Unknown"
        };

        private DiscountResponse Map(Discount d) => new DiscountResponse
        {
            DiscountId = d.DiscountId,
            Code = d.Code,
            FixValue = d.FixValue,
            PercentageValue = d.PercentageValue,
            MaxValue = d.MaxValue,
            MinPurchase = d.MinPurchase,
            Creator = d.Creator,
            CreatedAt = d.CreatedAt ?? string.Empty,
            StartDate = d.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
            EndDate = d.EndDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MaxValue,
            Status = GetStatusName(d.Status ?? 1),
            Used = d.Used ?? 0,
            Quantity = d.Quantity ?? 0,
            MaxUse = d.MaxUse ?? 1,
            Remaining = (d.Quantity ?? 0) - (d.Used ?? 0),
            Description = d.Description
        };
    }
}