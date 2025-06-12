using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class Language
{
    public int LanguageId { get; set; }

    public string? LanguageName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
