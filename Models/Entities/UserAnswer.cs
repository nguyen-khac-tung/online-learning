using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class UserAnswer
{
    public long UserAnswerId { get; set; }

    public string UserId { get; set; } = null!;

    public long QuestionId { get; set; }

    public long? OptionId { get; set; }

    public string? AnswerText { get; set; }

    public bool IsCorrect { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int Status { get; set; }

    public virtual Option? Option { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
