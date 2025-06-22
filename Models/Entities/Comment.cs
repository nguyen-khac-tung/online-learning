using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class Comment
{
    public long CommentId { get; set; }

    public long? ParentCommentId { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string UserId { get; set; } = null!;

    public long? LessonId { get; set; }

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Lesson? Lesson { get; set; }

    public virtual Comment? ParentComment { get; set; }

    public virtual User User { get; set; } = null!;
}
