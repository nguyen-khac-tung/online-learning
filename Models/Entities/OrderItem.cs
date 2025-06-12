using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class OrderItem
{
    public long OrderItemId { get; set; }

    public long OrderId { get; set; }

    public string? CourseId { get; set; }

    public decimal? Price { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Order Order { get; set; } = null!;
}
