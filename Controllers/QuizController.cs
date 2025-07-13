using Microsoft.AspNetCore.Mvc;
using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Services.Interfaces;
using System.Security.Claims;

namespace Online_Learning.Controllers
{
    [Route("api/quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        /// <summary>
        /// Lấy thông tin quiz theo ID
        /// </summary>
        /// <param name="quizId">ID của quiz</param>
        /// <returns>Thông tin quiz với danh sách câu hỏi và đáp án</returns>
        /// <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
        [HttpGet("{quizId}")]
        public async Task<ActionResult<QuizResponseDTO>> GetQuizByIdAsync(long quizId)
        {
            try
            {
                var quiz = await _quizService.GetQuizByIdAsync(quizId);
                if (quiz == null)
                {
                    return NotFound(ApiResponse<QuizResponseDTO>.ErrorResponse("Quiz không tồn tại"));
                }

                return Ok(ApiResponse<QuizResponseDTO>.SuccessResponse(quiz, "Lấy thông tin quiz thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<QuizResponseDTO>.ErrorResponse($"Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy danh sách quiz theo module
        /// </summary>
        /// <param name="moduleId">ID của module</param>
        /// <returns>Danh sách quiz của module</returns>
        /// /// <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
        [HttpGet("module/{moduleId}")]
        public async Task<ActionResult<IEnumerable<QuizResponseDTO>>> GetQuizzesByModuleAsync(long moduleId)
        {
            try
            {
                var quizzes = await _quizService.GetQuizzesByModuleAsync(moduleId);
                return Ok(ApiResponse<IEnumerable<QuizResponseDTO>>.SuccessResponse(quizzes, "Lấy danh sách quiz thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<QuizResponseDTO>>.ErrorResponse($"Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Nộp bài quiz
        /// </summary>
        /// <param name="request">Dữ liệu nộp bài</param>
        /// <returns>Kết quả quiz</returns>
        /// <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
        [HttpPost("submit")]
        public async Task<ActionResult<QuizResultResponseDTO>> SubmitQuizAsync([FromBody] QuizRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<QuizResultResponseDTO>.ErrorResponse("Chưa đăng nhập"));
                }

                var result = await _quizService.SubmitQuizAsync(request, userId);
                return Ok(ApiResponse<QuizResultResponseDTO>.SuccessResponse(result, "Nộp bài thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<QuizResultResponseDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<QuizResultResponseDTO>.ErrorResponse($"Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xem kết quả quiz
        /// </summary>
        /// <param name="quizId">ID của quiz</param>
        /// <returns>Kết quả chi tiết quiz</returns>
        ///  <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
        [HttpGet("result/{quizId}")]
        public async Task<ActionResult<QuizResultResponseDTO>> GetQuizResultAsync(long quizId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<QuizResultResponseDTO>.ErrorResponse("Chưa đăng nhập"));
                }

                var result = await _quizService.GetQuizResultAsync(quizId, userId);
                return Ok(ApiResponse<QuizResultResponseDTO>.SuccessResponse(result, "Lấy kết quả quiz thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<QuizResultResponseDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<QuizResultResponseDTO>.ErrorResponse($"Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy danh sách kết quả quiz của user
        /// </summary>
        /// <returns>Danh sách kết quả quiz</returns>
        ///  <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
        [HttpGet("results")]
        public async Task<ActionResult<IEnumerable<QuizResultResponseDTO>>> GetUserQuizResultsAsync()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<IEnumerable<QuizResultResponseDTO>>.ErrorResponse("Chưa đăng nhập"));
                }

                var results = await _quizService.GetUserQuizResultsAsync(userId);
                return Ok(ApiResponse<IEnumerable<QuizResultResponseDTO>>.SuccessResponse(results, "Lấy danh sách kết quả quiz thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<QuizResultResponseDTO>>.ErrorResponse($"Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Kiểm tra user đã hoàn thành quiz chưa
        /// </summary>
        /// <param name="quizId">ID của quiz</param>
        /// <returns>Trạng thái hoàn thành</returns>
        ///  <remarks>Author: HaiPDHE172178 | Role: STUDENT</remarks>
        [HttpGet("completed/{quizId}")]
        public async Task<ActionResult<bool>> IsQuizCompletedAsync(long quizId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<bool>.ErrorResponse("Chưa đăng nhập"));
                }

                var isCompleted = await _quizService.IsQuizCompletedAsync(quizId, userId);
                return Ok(ApiResponse<bool>.SuccessResponse(isCompleted, "Kiểm tra trạng thái quiz thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy userId từ token (tạm thời return null, cần implement authentication)
        /// </summary>
        /// <returns>UserId</returns>
        private string GetCurrentUserId()
        {
            // TODO: Implement authentication và lấy userId từ token
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Test
            return "U07de5297                           ";
        }
    }
} 