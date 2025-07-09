using Online_Learning.Constants.Enums;
using Online_Learning.Models.Entities;

namespace Online_Learning.Models.DTOs.Response.User
{
	public class CourseLearningResponseDTO
	{
		public string CourseId { get; set; } = null!;
		public string CourseName { get; set; } = null!;
		public int LessonQuantity { get; set; }
		public int Progress {  get; set; }
		public List<long> LessonIdCompleted { get; set; }
		public List<ModuleResponseDTO> Modules { get; set; } = new List<ModuleResponseDTO>();

		public CourseLearningResponseDTO()
		{
		}
	}
}
