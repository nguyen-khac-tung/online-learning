using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class UserQuizResult
{
    public long UserQuizResultId { get; set; }

    public string UserId { get; set; } = null!;

    public long QuizId { get; set; }

    public decimal Score { get; set; }

    public int TotalQuestions { get; set; }

    public int CorrectAnswers { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
