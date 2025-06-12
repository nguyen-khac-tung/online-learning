using System;
using System.Collections.Generic;

namespace Online_Learning.Models.Entities;

public partial class UserCertificate
{
    public int CertificateId { get; set; }

    public string UserId { get; set; } = null!;

    public string? CourseId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User User { get; set; } = null!;
}
