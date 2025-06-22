
namespace Online_Learning.Repositories.Interfaces
{
	public interface ICourseRepository
	{
		// USER - start
		// haipdhe
		IEnumerable<Models.DTOs.Response.User.CourseResponseDto> GetAllCourse();
		Models.DTOs.Response.User.CourseResponseDto GetCourseById(string id);
		// USER - end
	}
}
