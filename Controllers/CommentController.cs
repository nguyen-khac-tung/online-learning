
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Services.Interfaces;
using Online_Learning.Models.DTOs.Comment;
using Online_Learning.Models.DTOs.Common;
using System.Security.Claims;

namespace Online_Learning.Controllers
{
    [ApiController]
    [Route("api/comment")]
    //[Authorize] // Thêm authorization cho admin
    public class AdminCommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<AdminCommentController> _logger;

        public AdminCommentController(ICommentService commentService, ILogger<AdminCommentController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        // GET /api/comment - List comments với filters cho admin
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<CommentDto>>>> GetComments([FromQuery] CommentFilterRequest request)
        {
            try
            {
                var result = await _commentService.GetCommentsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments with filters");
                return StatusCode(500, ApiResponse<PagedResult<CommentDto>>.ErrorResponse("Internal server error"));
            }
        }

        // GET /api/comment/statistics - Thống kê comment
        [HttpGet("statistics")]
        public async Task<ActionResult<ApiResponse<CommentStatisticsDto>>> GetStatistics()
        {
            try
            {
                var result = await _commentService.GetCommentStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment statistics");
                return StatusCode(500, ApiResponse<CommentStatisticsDto>.ErrorResponse("Internal server error"));
            }
        }

        // POST /api/comment/moderate/bulk - Bulk moderation
        [HttpPost("moderate/bulk")]
        public async Task<ActionResult<ApiResponse<bool>>> BulkModerateComments([FromBody] BulkCommentModerationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request"));
                }

                //var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //if (string.IsNullOrEmpty(adminId))
                //{
                //    return Unauthorized(ApiResponse<bool>.ErrorResponse("Admin not authenticated"));
                //}

                //fake admin 
                var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Fake để dev
                if (string.IsNullOrEmpty(adminId))
                {
                    adminId = "fake-admin-id";
                }
                var result = await _commentService.BulkModerateCommentsAsync(request, adminId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk moderation");
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
            }
        }

        // PUT /api/comment/moderate/bulk - Alternative endpoint cho bulk moderation
        [HttpPut("moderate/bulk")]
        public async Task<ActionResult<ApiResponse<bool>>> BulkModerateCommentsPut([FromBody] BulkCommentModerationRequest request)
        {
            return await BulkModerateComments(request);
        }

        // GET /api/comment/pending - Lấy comments đang chờ duyệt
        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse<PagedResult<CommentDto>>>> GetPendingComments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _commentService.GetPendingCommentsAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending comments");
                return StatusCode(500, ApiResponse<PagedResult<CommentDto>>.ErrorResponse("Internal server error"));
            }
        }

        // POST /api/comment/moderate - Single comment moderation
        [HttpPost("moderate")]
        public async Task<ActionResult<ApiResponse<bool>>> ModerateComment([FromBody] CommentModerationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request"));
                }

                //var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //if (string.IsNullOrEmpty(adminId))
                //{
                //    return Unauthorized(ApiResponse<bool>.ErrorResponse("Admin not authenticated"));
                //}
                var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(adminId))
                {
                    adminId = "fake-admin-id";
                }
                var result = await _commentService.ModerateCommentAsync(request, adminId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moderating comment");
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
            }
        }
    }
}