using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.Entities;

public partial class Course
{
    [Key]
    public string CourseId { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string? Acceptor { get; set; }

    [Required]
    public string Creator { get; set; } = string.Empty;
    public int Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [Required]
    public string StudyTime { get; set; } = string.Empty;
    public int? LevelId { get; set; }

    public int? LanguageId { get; set; }

    public int CertificateId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<CourseCategory> CourseCategories { get; set; } = new List<CourseCategory>();

    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public virtual ICollection<CourseImage> CourseImages { get; set; } = new List<CourseImage>();

    public virtual ICollection<CoursePrice> CoursePrices { get; set; } = new List<CoursePrice>();

    public virtual Language? Language { get; set; }

    public virtual Level? Level { get; set; }

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<UserCertificate> UserCertificates { get; set; } = new List<UserCertificate>();
}
