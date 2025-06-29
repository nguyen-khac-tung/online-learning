using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.Entities;
using Online_Learning.Services.Interfaces;
using System.Runtime.CompilerServices;

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

		/// <summary>
		/// Get list of courses with filter, search, sort and pagination (accessible by guest or user)
		/// </summary>
		/// <param name="request">Filter parameters</param>
		/// <remarks>Author: HaiPDHE172178 | Role: USER</remarks>
		[HttpGet("filter")]
		public async Task<ActionResult<PaginatedResponse<CourseResponseDTO>>> GetCoursesWithFilterAsync([FromQuery] CourseRequestDto request)
		{
			var result = await _courseService.GetCoursesWithFilterAsync(request);

			if (result == null || !result.DataPaginated.Any())
			{
				return NotFound(ApiResponse<PaginatedResponse<CourseResponseDTO>>.NotFoundResponse("No courses found"));
			}

			return Ok(ApiResponse<PaginatedResponse<CourseResponseDTO>>.SuccessResponse(result, "Courses retrieved successfully"));
		}

		/// <summary>
		/// Get detail of a specific course by ID (accessible by guest or user)
		/// </summary>
		/// <param name="id">Course ID</param>
		/// <remarks>Author: HaiPDHE172178 | Role: USER</remarks>
		[HttpGet("{id}")]
		public async Task<ActionResult<CourseResponseDTO>> GetCourseByIdAsync(string id)
		{
			var course = await _courseService.GetCourseByIdAsync(id);

			if (course == null)
			{
				return NotFound(ApiResponse<IEnumerable<CourseResponseDTO>>.NotFoundResponse("No courses found"));
			}

			return Ok(ApiResponse<CourseResponseDTO>.SuccessResponse(course, "Courses retrieved successfully"));
		}

		/// <summary>
		/// Get course in my learning by UserID (accessible by user)
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <remarks>Author: HaiPDHE172178 | Role: USER</remarks>
		[HttpGet("my-learning")]
		public async Task<ActionResult<IEnumerable<CourseProgressResponseDTO>>> GetMyLearningCourseAsync([FromQuery] string userId)
		{
			var courses = await _courseService.GetCourseProgressByUserIdAsync(userId);
			if (courses == null || !courses.Any())
			{
				return NotFound(ApiResponse<IEnumerable<CourseProgressResponseDTO>>.NotFoundResponse("No courses found"));
			}

			return Ok(ApiResponse<IEnumerable<CourseProgressResponseDTO>>.SuccessResponse(courses, "Courses retrieved successfully"));
		}

		/// <summary>
		/// Mark as completed lesson (accessible by user)
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="lessonId">Lesson ID</param>
		/// <remarks>Author: HaiPDHE172178 | Role: USER</remarks>
		[HttpPost("mark-as-completed")]
		public async Task<ActionResult> UpdateProgressLesson(string userId, long lessonId)
		{
			_courseService.UpdateLessonProgress(userId, lessonId);
			return Ok(ApiResponse<string>.SuccessResponse(null, "Lesson marked as completed"));
		}


	}
}
