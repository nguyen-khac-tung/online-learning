using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class UserAnswer
{
    public long UserAnswerId { get; set; }

    public string UserId { get; set; } = null!;

    public long OptionId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Option Option { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
