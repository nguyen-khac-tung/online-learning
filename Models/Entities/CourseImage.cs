using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class CourseImage
{
    public long ImageId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
