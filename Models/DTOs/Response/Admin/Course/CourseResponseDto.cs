using System;
using System.Collections.Generic;

namespace Online_Learning.Models.DTOs.Response.Admin.Course
{

    public class CourseResponseDto
    {

        public string CourseID { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Acceptor { get; set; }

        public string Creator { get; set; } = string.Empty;

        public int Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string StudyTime { get; set; } = string.Empty;

        public int? LevelID { get; set; }

        public int? LanguageID { get; set; }

        // public int CertificateID { get; set; } // Removed

        public List<string> ImageUrls { get; set; } = new List<string>();

        public int ModuleCount { get; set; }

        public decimal? CurrentPrice { get; set; }

        public List<CoursePriceResponseDto> PriceHistory { get; set; } = new List<CoursePriceResponseDto>();

        public List<int> CategoryIDs { get; set; } = new List<int>();

    }

}