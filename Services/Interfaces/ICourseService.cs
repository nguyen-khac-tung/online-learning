using Online_Learning.Models.DTOs.Response.User;

namespace Online_Learning.Services.Interfaces
{
	public interface ICourseService
	{
		IEnumerable<CourseResponseDto> GetAllCourse();
		CourseResponseDto GetCourseById(string id);
	}
}
