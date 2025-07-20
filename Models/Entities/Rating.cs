using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Online_Learning.Models.Entities;

[Index("CourseId", Name = "IX_Ratings_CourseID")]
[Index("UserId", "CourseId", Name = "UQ_Ratings_User_Course", IsUnique = true)]
public partial class Rating
{
    [Key]
    [Column("RatingID")]
    public long RatingId { get; set; }

    [Column("CourseID")]
    [StringLength(36)]
    [Unicode(false)]
    public string CourseId { get; set; } = null!;

    [Column("UserID")]
    [StringLength(36)]
    [Unicode(false)]
    public string UserId { get; set; } = null!;

    public byte Score { get; set; }

    public string? ReviewText { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    
    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
