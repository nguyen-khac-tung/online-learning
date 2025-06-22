using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Controllers
{
	[Route("api/courses")]
	[ApiController]
	public class CoursesController : ControllerBase
	{
		private readonly ICourseService _courseService;
		public CoursesController(ICourseService courseService)
		{
			_courseService = courseService;
		}
		[HttpGet]
		public ActionResult<IEnumerable<CourseResponseDto>> GetAllCourses()
		{
			var courses = _courseService.GetAllCourse();

			if (courses == null || !courses.Any())
			{
				return NotFound(ApiResponse<IEnumerable<CourseResponseDto>>.NotFoundResponse("No courses found"));
			}

			return Ok(ApiResponse<IEnumerable<CourseResponseDto>>.SuccessResponse(courses, "Courses retrieved successfully"));
		}
	}
}
