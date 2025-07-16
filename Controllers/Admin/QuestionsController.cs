using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Learning.Models;
using Online_Learning.Models.DTOs.Request.Admin.QuestionDto;
using Online_Learning.Models.DTOs.Response.Admin.QuestionDto;
using Online_Learning.Models.DTOs.Request.Admin.OptionDto;
using Online_Learning.Models.DTOs.Response.Admin.OptionDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Online_Learning.Models.Entities;
using Online_Learning.Services.Interfaces;
using Online_Learning.Services.Interfaces.Admin;

namespace Online_Learning.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        // GET: api/Questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionResponseDto>>> GetQuestions([FromQuery] long quizId)
        {
            var questions = await _questionService.GetQuestionsByQuizAsync(quizId);
            return Ok(questions);
        }

        // GET: api/Questions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionResponseDto>> GetQuestion(long id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        // POST: api/Questions
        [HttpPost]
        public async Task<ActionResult<QuestionResponseDto>> CreateQuestion(QuestionCreateDto questionDto)
        {
            var question = await _questionService.CreateQuestionAsync(questionDto.QuizID, questionDto);
            return CreatedAtAction(nameof(GetQuestion), new { id = question.QuestionID }, question);
        }

        // PUT: api/Questions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(long id, QuestionUpdateDto questionDto)
        {
            var result = await _questionService.UpdateQuestionAsync(id, questionDto);
            if (!result) return NotFound();
            return NoContent();
        }

        // DELETE: api/Questions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(long id)
        {
            var result = await _questionService.DeleteQuestionAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
