using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.DTOs.Request.Admin.OptionDto;
using Online_Learning.Models.DTOs.Response.Admin.OptionDto;
using Online_Learning.Models.Entities;
using Online_Learning.Services.Interfaces;
using Online_Learning.Services.Interfaces.Admin;

namespace Online_Learning.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class OptionsController : ControllerBase
    {
        private readonly IOptionService _optionService;

        public OptionsController(IOptionService optionService)
        {
            _optionService = optionService;
        }

        // GET: api/Options
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OptionResponseDto>>> GetOptions([FromQuery] long questionId)
        {
            var options = await _optionService.GetOptionsByQuestionAsync(questionId);
            return Ok(options);
        }

        // GET: api/Options/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OptionResponseDto>> GetOption(long id)
        {
            var option = await _optionService.GetOptionByIdAsync(id);
            if (option == null) return NotFound();
            return Ok(option);
        }

        // POST: api/Options
        [HttpPost]
        public async Task<ActionResult<OptionResponseDto>> CreateOption([FromQuery] long questionId, OptionCreateDto optionDto)
        {
            var option = await _optionService.CreateOptionAsync(questionId, optionDto);
            return CreatedAtAction(nameof(GetOption), new { id = option.OptionID }, option);
        }

        // PUT: api/Options/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOption(long id, OptionUpdateDto optionDto)
        {
            var result = await _optionService.UpdateOptionAsync(id, optionDto);
            if (!result) return NotFound();
            return NoContent();
        }

        // DELETE: api/Options/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOption(long id)
        {
            var result = await _optionService.DeleteOptionAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
