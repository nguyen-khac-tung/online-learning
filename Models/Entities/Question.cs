using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class Question
{
    public long QuestionId { get; set; }

    public long QuizId { get; set; }

    public int QuestionNum { get; set; }

    public string Content { get; set; } = null!;

    public int Type { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Option> Options { get; set; } = new List<Option>();

    public virtual Quiz Quiz { get; set; } = null!;
}
