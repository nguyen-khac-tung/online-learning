using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class Order
{
    public long OrderId { get; set; }

    public string PaymetMethod { get; set; } = null!;

    public float TotalAmount { get; set; }

    public DateTime OrderDate { get; set; }

    public string UserId { get; set; } = null!;

    public long? DiscountId { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
