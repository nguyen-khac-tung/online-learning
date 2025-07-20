using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.DTOs.Request.Admin.QuestionDto;
using Online_Learning.Models.DTOs.Request.Admin.QuizDto;
using Online_Learning.Models.DTOs.Response.Admin.OptionDto;
using Online_Learning.Models.DTOs.Response.Admin.QuestionDto;
using Online_Learning.Models.DTOs.Response.Admin.QuizDto;
using Online_Learning.Models.Entities;
using Online_Learning.Services.Interfaces.Admin;

namespace Online_Learning.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class QuizzesController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILogger<QuizzesController> _logger;

        public QuizzesController(IQuizService quizService, ILogger<QuizzesController> logger)
        {
            _quizService = quizService;
            _logger = logger;
        }

        // GET: api/Quizzes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizResponseDto>>> GetQuizzes(
            [FromQuery] long? moduleId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var quizzes = await _quizService.GetQuizzesAsync(moduleId, page, pageSize);

            var totalCount = await _quizService.GetTotalCountAsync(moduleId);
            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return Ok(quizzes);
        }

        // GET: api/Quizzes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuizResponseDto>> GetQuiz(long id)
        {
            var quiz = await _quizService.GetQuizByIdAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }
            return Ok(quiz);
        }

        // POST: api/Quizzes
        [HttpPost]
        public async Task<ActionResult<QuizResponseDto>> CreateQuiz(QuizCreateDto quizDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await _quizService.CreateQuizAsync(quizDto);
                return CreatedAtAction(nameof(GetQuiz), new { id = response.QuizID }, response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create quiz {QuizName}", quizDto.QuizName);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Quizzes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuiz(long id, QuizUpdateDto quizDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _quizService.UpdateQuizAsync(id, quizDto);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update quiz {QuizId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Quizzes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(long id)
        {
            var result = await _quizService.DeleteQuizAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        //// POST: api/Quizzes/5/questions
        //[HttpPost("{quizId}/questions")]
        //public async Task<ActionResult<QuestionResponseDto>> AddQuestion(long quizId, QuestionCreateDto questionDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var quiz = await _context.Quizzes.FindAsync(quizId);
        //    if (quiz == null)
        //    {
        //        return NotFound("Quiz not found");
        //    }

        //    using var transaction = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var question = new Question
        //        {
        //            QuizId = quizId,
        //            QuestionNum = questionDto.QuestionNum,
        //            Content = questionDto.Content,
        //            Type = questionDto.Type,
        //            CreatedAt = DateTime.UtcNow,
        //            Status = 1
        //        };

        //        _context.Questions.Add(question);
        //        await _context.SaveChangesAsync();

        //        foreach (var optionDto in questionDto.Options)
        //        {
        //            var option = new Option
        //            {
        //                QuestionID = question.QuestionId,
        //                Content = optionDto.Content,
        //                IsCorrect = optionDto.IsCorrect,
        //                CreatedAt = DateTime.UtcNow,
        //                Status = 1
        //            };

        //            _context.Options.Add(option);
        //        }

        //        await _context.SaveChangesAsync();

        //        // Update quiz total questions count
        //        quiz.TotalQuestions = await _context.Questions.CountAsync(q => q.QuizId == quizId);
        //        await _context.SaveChangesAsync();

        //        await transaction.CommitAsync();

        //        var response = new QuestionResponseDto
        //        {
        //            QuestionID = question.QuestionId,
        //            QuizID = question.QuizId,
        //            QuestionNum = question.QuestionNum,
        //            Content = question.Content,
        //            Type = question.Type,
        //            CreatedAt = question.CreatedAt,
        //            UpdatedAt = question.UpdatedAt,
        //            Status = question.Status,
        //            Options = await _context.Options
        //                .Where(o => o.QuestionID == question.QuestionId)
        //                .Select(o => new OptionResponseDto
        //                {
        //                    OptionID = o.OptionId,
        //                    QuestionID = o.QuestionID,
        //                    Content = o.Content,
        //                    IsCorrect = o.IsCorrect,
        //                    CreatedAt = o.CreatedAt,
        //                    UpdatedAt = o.UpdatedAt,
        //                    Status = o.Status
        //                }).ToListAsync()
        //        };

        //        return Ok(response);
        //    }
        //    catch (Exception)
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}

        // GET: api/Quizzes/module/5
        [HttpGet("module/{moduleId}")]
        public async Task<ActionResult<IEnumerable<QuizResponseDto>>> GetQuizzesByModule(long moduleId)
        {
            var quizzes = await _quizService.GetQuizzesByModuleAsync(moduleId);
            return Ok(quizzes);
        }

        //private bool QuizExists(long id)
        //{
        //    return _context.Quizzes.Any(e => e.QuizId == id);
        //}
    }
}
