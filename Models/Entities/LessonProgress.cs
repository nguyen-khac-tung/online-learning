using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class LessonProgress
{
    public string UserId { get; set; } = null!;

    public long LessonId { get; set; }

    public DateTime CompletedAt { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
