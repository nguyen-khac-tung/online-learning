﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.Entities;
using Online_Learning.Services.Interfaces;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
		/// <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
		[HttpGet("filter")]
		public async Task<ActionResult<PaginatedResponse<CourseResponseDTO>>> GetCoursesWithFilterAsync([FromQuery] CourseRequestDto request)
		{
			var result = await _courseService.GetCoursesWithFilterAsync(request);
			return Ok(ApiResponse<PaginatedResponse<CourseResponseDTO>>.SuccessResponse(result, "Courses retrieved successfully"));
		}

		/// <summary>
		/// Get detail of a specific course by ID (accessible by guest or user)
		/// </summary>
		/// <param name="id">Course ID</param>
		/// <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
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
		/// <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
		// [Authorize(Roles ="Student")]	
		[Authorize(Roles = "Student")]
		[HttpGet("my-learning/{progress}")]
		public async Task<ActionResult<IEnumerable<CourseProgressResponseDTO>>> GetMyLearningCourseAsync(string progress)
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(ApiResponse<string>.UnauthorizedResponse("User is not authenticated"));
			}

			var courses = await _courseService.GetCourseProgressByUserIdAsync(userId, progress);
			return Ok(ApiResponse<IEnumerable<CourseProgressResponseDTO>>.SuccessResponse(courses, "Courses retrieved successfully"));
		}


		/// <summary>
		/// Mark as completed lesson (accessible by user)
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="lessonId">Lesson ID</param>
		/// <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
		[Authorize(Roles = "Student")]
		[HttpPost("mark-as-completed/{lessonId}")]
		public async Task<ActionResult> UpdateProgressLesson(long lessonId)
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(ApiResponse<string>.UnauthorizedResponse("User is not authenticated"));
			}
			_courseService.UpdateLessonProgress(userId, lessonId);
			return Ok(ApiResponse<string>.SuccessResponse(null, "Lesson marked as completed"));
		}

		/// <summary>
		/// Check if user is enrolled in a course
		/// </summary>
		/// <param name="courseId">Course ID</param>
		/// <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
		[HttpGet("enrollment/{courseId}")]
		public async Task<ActionResult<bool>> CheckEnrollmentAsync(string courseId)
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated"));
			}

			var isEnrolled = await _courseService.CheckEnrollmentAsync(userId, courseId);
			return Ok(ApiResponse<bool>.SuccessResponse(isEnrolled, "Check enrollment successful"));
		}

		/// <summary>
		/// Get detailed course progress for learning
		/// </summary>
		/// <param name="courseId">Course ID</param>
		/// <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
		[Authorize(Roles = "Student")]
		[HttpGet("learning/{courseId}")]
		public async Task<ActionResult<CourseLearningResponseDTO>> GetCourseLearningAsync(string courseId)
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(ApiResponse<CourseLearningResponseDTO>.ErrorResponse("User is not authenticated"));
			}

			var courseLearning = await _courseService.GetCourseLearningAsync(courseId, userId);
			if (courseLearning == null)
			{
				return NotFound(ApiResponse<CourseLearningResponseDTO>.ErrorResponse("Couse not found"));
			}

			return Ok(ApiResponse<CourseLearningResponseDTO>.SuccessResponse(courseLearning, "Get course successful"));
		}
		/// <summary>
		/// Get detailed course progress for learning
		/// </summary>
		/// <param name="courseId">Course ID</param>
		/// <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
		[Authorize(Roles = "Student")]
		[HttpGet("progress/{courseId}")]
		public async Task<ActionResult<int>> GetProgressAsync(string courseId)
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(ApiResponse<int>.ErrorResponse("User is not authenticated"));
			}

			var progress = await _courseService.GetProgressAsync(userId, courseId);
			return Ok(ApiResponse<int>.SuccessResponse(progress, "Get progress successful"));
		}

		/// <summary>
		/// Đăng ký khóa học (chỉ cho phép Student)
		/// </summary>
		/// <param name="courseId">ID khóa học</param>
		/// <returns>Kết quả đăng ký</returns>
		/// <remarks>Author: GPT | Role: STUDENT</remarks>
		[Authorize(Roles = "Student")]
		[HttpPost("enroll/{courseId}")]
		public async Task<ActionResult> EnrollCourseAsync(string courseId)
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(ApiResponse<string>.UnauthorizedResponse("User is not authenticated"));
			}
			var result = await _courseService.EnrollCourseAsync(userId, courseId);
			if (!result)
			{
				return BadRequest(ApiResponse<string>.ErrorResponse("Đăng ký khóa học thất bại hoặc đã đăng ký trước đó"));
			}
			return Ok(ApiResponse<string>.SuccessResponse(null, "Đăng ký khóa học thành công"));
		}

		/// <summary>
		/// Get current user ID from token (temporary implementation)
		/// </summary>
		/// <returns>User ID</returns>
		private string GetCurrentUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}

	}
}
