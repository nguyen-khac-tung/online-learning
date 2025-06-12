using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class DiscussionLesson
{
    public long DiscussionId { get; set; }

    public long? ParentCommentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Comment { get; set; } = null!;

    public string? UserId { get; set; }

    public long? LessonId { get; set; }

    public virtual ICollection<DiscussionLesson> InverseParentComment { get; set; } = new List<DiscussionLesson>();

    public virtual Lesson? Lesson { get; set; }

    public virtual DiscussionLesson? ParentComment { get; set; }

    public virtual User? User { get; set; }
}
