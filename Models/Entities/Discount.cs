using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class Discount
{
    public long DiscountId { get; set; }

    public string Code { get; set; } = null!;

    public decimal? FixValue { get; set; }

    public double? PercentageValue { get; set; }

    public decimal? MaxValue { get; set; }

    public decimal? MinPurchase { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? Quantity { get; set; }

    public int? MaxUse { get; set; }

    public int? Used { get; set; }

    public string? Description { get; set; }

    public string? CreatedAt { get; set; }

    public string Creator { get; set; } = null!;

    public int? Status { get; set; }

    public virtual User CreatorNavigation { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
