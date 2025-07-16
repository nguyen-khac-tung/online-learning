using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.DTOs.Request.Admin.ModuleDto;
using Online_Learning.Models.DTOs.Response.Admin.ModuleDto;
using Online_Learning.Models.Entities;
using Online_Learning.Services.Interfaces;
using Online_Learning.Services.Interfaces.Admin;

namespace Online_Learning.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly ILogger<ModulesController> _logger;

        public ModulesController(IModuleService moduleService, ILogger<ModulesController> logger)
        {
            _moduleService = moduleService;
            _logger = logger;
        }

        // GET: api/Modules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleResponseDto>>> GetModules(
            [FromQuery] string? courseId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var modules = await _moduleService.GetModulesAsync(courseId, page, pageSize);
            var totalCount = await _moduleService.GetTotalCountAsync(courseId);
            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return Ok(modules);
        }

        // GET: api/Modules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModuleResponseDto>> GetModule(long id)
        {
            var module = await _moduleService.GetModuleByIdAsync(id);
            if (module == null)
            {
                return NotFound();
            }
            return Ok(module);
        }

        // POST: api/Modules
        [HttpPost]
        public async Task<ActionResult<ModuleResponseDto>> CreateModule(ModuleCreateDto moduleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await _moduleService.CreateModuleAsync(moduleDto);
                return CreatedAtAction(nameof(GetModule), new { id = response.ModuleID }, response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create module {ModuleName}", moduleDto.ModuleName);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Modules/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModule(long id, ModuleUpdateDto moduleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _moduleService.UpdateModuleAsync(id, moduleDto);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update module {ModuleId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Modules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(long id)
        {
            try
            {
                var result = await _moduleService.DeleteModuleAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete module {ModuleId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Modules/course/5
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<ModuleResponseDto>>> GetModulesByCourse(string courseId)
        {
            var modules = await _moduleService.GetModulesByCourseAsync(courseId);
            return Ok(modules);
        }
    }
}
