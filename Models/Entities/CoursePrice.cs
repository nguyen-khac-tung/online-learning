using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Learning.Models.Entities;

public partial class CoursePrice
{
    [Key]
    public long CoursePriceId { get; set; }
    [Required]
    public string CourseId { get; set; } = null!;
    [Required]
    [Column(TypeName = "money")]
    public decimal Price { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public virtual Course Course { get; set; } = null!;
}
