using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class CartItem
{
    public long CartItemId { get; set; }

    public string UserId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
