using Online_Learning.Constants.Enums;
using Online_Learning.Models.Entities;

namespace Online_Learning.Models.DTOs.Response.User
{
	public class CourseProgressResponseDTO
	{
		public string CourseId { get; set; } = null!;

		public string CourseName { get; set; } = null!;

		public CourseStatus Status { get; set; }

		public string CourseImgUrl { get; set; } = null!;

		public int PercentCompleted { get; set; } = 0;

		public CourseProgressResponseDTO(Course course, int percent)
		{
			CourseId = course.CourseId;
			CourseName = course.CourseName;
			CourseImgUrl = course.CourseImages
					.OrderByDescending(c => c.ImageId)
					.Select(c => c.ImageUrl)
					.FirstOrDefault();
			PercentCompleted = percent;
		}
	}
}
