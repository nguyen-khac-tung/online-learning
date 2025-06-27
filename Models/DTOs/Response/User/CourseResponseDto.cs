using Online_Learning.Constants.Enums;
using Online_Learning.Models.Entities;

namespace Online_Learning.Models.DTOs.Response.User
{
	public class CourseResponseDTO
	{
		public string CourseId { get; set; } = null!;

		public string CourseName { get; set; } = null!;

		public string? Description { get; set; }

		public CourseStatus Status { get; set; }

		public string StudyTime { get; set; } = null!;
		public string CourseImgUrl { get; set; } = null!;

		public string LevelName { get; set; }

		public string Language { get; set; }

		public List<ModuleResponseDTO> Modules { get; set; } = new List<ModuleResponseDTO>();
		public decimal Price { get; set; }

		public CourseResponseDTO(Course course)
		{
			CourseId = course.CourseId;
			CourseName = course.CourseName;
			Description = course.Description;
			Status = (CourseStatus)course.Status;
			StudyTime = course.StudyTime;
			LevelName = course.Level.LevelName;
			Language = course.Language.LanguageName;
			if (course.Modules != null)
			{
				Modules = course.Modules
					.Where(m => m.Status == (int)ModuleStatus.Active)
					.OrderBy(m => m.ModuleNumber)
					.Select(m => new ModuleResponseDTO(m))
					.ToList();
			}
			Price = (course.CoursePrices != null) ? (course.CoursePrices
				.OrderByDescending(c => c.CreateAt)
				.Select(c => c.Price)
				.FirstOrDefault()) : 0;
			CourseImgUrl = course.CourseImages
					.OrderByDescending(c => c.ImageId)
					.Select(c => c.ImageUrl)
					.FirstOrDefault(); 
		}
	}
}
