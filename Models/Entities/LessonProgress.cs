using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class LessonProgress
{
    public long LessonId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime? CompletedAt { get; set; }

    public bool? IsCompleted { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
