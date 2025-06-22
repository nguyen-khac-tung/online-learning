using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class Module
{
    public long ModuleId { get; set; }

    public string ModuleName { get; set; } = null!;

    public int ModuleNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int Status { get; set; }

    public string CourseId { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
