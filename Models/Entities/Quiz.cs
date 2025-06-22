using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class Quiz
{
    public long QuizId { get; set; }

    public long ModuleId { get; set; }

    public string QuizName { get; set; } = null!;

    public int? QuizTime { get; set; }

    public int TotalQuestions { get; set; }

    public int? PassScore { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int Status { get; set; }

    public virtual Module Module { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<UserQuizResult> UserQuizResults { get; set; } = new List<UserQuizResult>();
}
