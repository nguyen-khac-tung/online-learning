using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class Course
{
    public string CourseId { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Acceptor { get; set; }

    public string Creator { get; set; } = null!;

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string StudyTime { get; set; } = null!;

    public int? LevelId { get; set; }

    public int? LanguageId { get; set; }

    public int CertificateId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<CourseCategory> CourseCategories { get; set; } = new List<CourseCategory>();

    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public virtual ICollection<CourseImage> CourseImages { get; set; } = new List<CourseImage>();

    public virtual Language? Language { get; set; }

    public virtual Level? Level { get; set; }

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<UserCertificate> UserCertificates { get; set; } = new List<UserCertificate>();
}
