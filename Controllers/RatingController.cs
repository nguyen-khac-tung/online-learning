using Microsoft.AspNetCore.Mvc;
using Online_Learning.Models.DTOs.Rating;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _svc;

        public RatingsController(IRatingService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRatingRequest req)
        {
            try
            {
                var result = await _svc.CreateAsync(req.UserId, req);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourse(string courseId)
        {
            var list = await _svc.GetByCourseAsync(courseId);
            return Ok(list);
        }

        [HttpGet("course/{courseId}/average")]
        public async Task<IActionResult> GetAverage(string courseId)
        {
            var avg = await _svc.GetAverageAsync(courseId);
            return Ok(new { average = avg });
        }

        [HttpGet("course/{courseId}/user/{userId}")]
        public async Task<IActionResult> GetUserRatingForCourse(string courseId, string userId)
        {
            var rating = await _svc.GetByUserCourseAsync(userId, courseId);
            if (rating == null)
                return NotFound(new { message = "Người dùng chưa đánh giá khóa học này." });
            return Ok(rating);
        }

        [HttpDelete("{ratingId}")]
        public async Task<IActionResult> DeleteRating(long ratingId)
        {
            var success = await _svc.DeleteAsync(ratingId);
            if (!success)
                return NotFound(new { message = "Không tìm thấy đánh giá." });
            return Ok(new { message = "Xóa đánh giá thành công." });
        }

        [HttpGet("course/{courseId}/paged")]
        public async Task<IActionResult> GetByCoursePaged(string courseId, int page = 1, int pageSize = 10)
        {
            var (ratings, totalCount) = await _svc.GetByCoursePagedAsync(courseId, page, pageSize);
            return Ok(new { data = ratings, totalCount, page, pageSize });
        }
    }
}
