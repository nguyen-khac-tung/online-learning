using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.DTOs.Request.Admin.LessonDto;
using Online_Learning.Models.DTOs.Response.Admin.LessonDto;
using Online_Learning.Models.Entities;
using Online_Learning.Services;
using Online_Learning.Services.Interfaces;
using Online_Learning.Services.Interfaces.Admin;

namespace Online_Learning.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly ILogger<LessonsController> _logger;

        public LessonsController(ILessonService lessonService, ILogger<LessonsController> logger)
        {
            _lessonService = lessonService;
            _logger = logger;
        }

        // GET: api/Lessons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonResponseDto>>> GetLessons(
            [FromQuery] long? moduleId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var lessons = await _lessonService.GetLessonsAsync(moduleId, page, pageSize);
            var totalCount = await _lessonService.GetTotalCountAsync(moduleId);
            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return Ok(lessons);
        }

        // GET: api/Lessons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LessonResponseDto>> GetLesson(long id)
        {
            var lesson = await _lessonService.GetLessonByIdAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            return Ok(lesson);
        }

        // POST: api/Lessons
        [HttpPost]
        public async Task<ActionResult<LessonResponseDto>> CreateLesson(LessonCreateDto lessonDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await _lessonService.CreateLessonAsync(lessonDto);
                return CreatedAtAction(nameof(GetLesson), new { id = response.LessonID }, response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create lesson {LessonName}", lessonDto.LessonName);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Lessons/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLesson(long id, LessonUpdateDto lessonDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _lessonService.UpdateLessonAsync(id, lessonDto);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update lesson {LessonId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Lessons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(long id)
        {
            var result = await _lessonService.DeleteLessonAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        // GET: api/Lessons/module/5
        [HttpGet("module/{moduleId}")]
        public async Task<ActionResult<IEnumerable<LessonResponseDto>>> GetLessonsByModule(long moduleId)
        {
            var lessons = await _lessonService.GetLessonsByModuleAsync(moduleId);
            return Ok(lessons);
        }
    }
}
