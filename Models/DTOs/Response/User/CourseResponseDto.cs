using Online_Learning.Models.Entities;

namespace Online_Learning.Models.DTOs.Response.User
{
	public class CourseResponseDto
	{
		public string CourseId { get; set; } = null!;

		public string CourseName { get; set; } = null!;

		public string? Description { get; set; }

		public int Status { get; set; }

		public string StudyTime { get; set; } = null!;

		public string LevelName { get; set; }

		public string Language { get; set; }

		public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
		public virtual ICollection<CoursePrice> CoursePrices { get; set; } = new List<CoursePrice>();

		public CourseResponseDto()
		{

		}
		public CourseResponseDto(Course course)
		{
			CourseId = course.CourseId;
			CourseName = course.CourseName;
			Description = course.Description;
			Status = course.Status;
			StudyTime = course.StudyTime;
			LevelName = course.Level.LevelName;
			Language = course.Language.LanguageName;

		}
	}
}
