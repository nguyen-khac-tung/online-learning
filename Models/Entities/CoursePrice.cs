using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class CoursePrice
{
    public long CoursePriceId { get; set; }

    public string CourseId { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual Course Course { get; set; } = null!;
}
