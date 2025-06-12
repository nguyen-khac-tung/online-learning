using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class CourseCategory
{
    public int CategoryId { get; set; }

    public string CourseId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
