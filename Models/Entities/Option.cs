﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class Option
{
    public long OptionId { get; set; }

    public long QuestionId { get; set; }

    public string Content { get; set; }

    public bool IsCorrect { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int Status { get; set; }

    public virtual Question Question { get; set; }

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}