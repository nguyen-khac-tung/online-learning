using Online_Learning.Constants.Enums;
using Online_Learning.Models.Entities;

namespace Online_Learning.Models.DTOs.Response.User
{
	public class LessonResponseDTO
	{
		public long LessonId { get; set; }
		public string LessonName { get; set; } = null!;
		public int LessonNumber { get; set; }
		public int? Duration { get; set; }
		public LessonStatus Status { get; set; }
		public string StatusName => Status.ToString();

		public LessonResponseDTO(Lesson lesson)
		{
			LessonId = lesson.LessonId;
			LessonName = lesson.LessonName;
			LessonNumber = lesson.LessonNumber;
			Duration = lesson.Duration;
			Status = (LessonStatus)(lesson.Status ?? 0);
		}
	}
}
