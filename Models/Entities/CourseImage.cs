using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.Entities;

public partial class CourseImage
{
    [Key]
    public long ImageID { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    public string CourseID { get; set; } = string.Empty;

    public virtual Course Course { get; set; } = null!;
}
