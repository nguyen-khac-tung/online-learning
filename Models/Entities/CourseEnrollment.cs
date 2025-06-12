using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class CourseEnrollment
{
    public string CourseId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int Status { get; set; }

    public int Progress { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
