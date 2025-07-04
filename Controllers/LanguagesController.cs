using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguagesController : ControllerBase
    {
        private readonly OnlineLearningContext _context;
        private readonly ILogger<LanguagesController> _logger;

        public LanguagesController(OnlineLearningContext context, ILogger<LanguagesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetLanguages()
        {
            try
            {
                var languages = await _context.Languages
                    .Where(l => l.DeletedAt == null)
                    .Select(l => new
                    {
                        l.LanguageId,
                        l.LanguageName,
                        l.Status,
                        l.CreatedAt,
                        l.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(languages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving languages");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetLanguage(int id)
        {
            try
            {
                var language = await _context.Languages
                    .Where(l => l.DeletedAt == null && l.LanguageId == id)
                    .Select(l => new
                    {
                        l.LanguageId,
                        l.LanguageName,
                        l.Status,
                        l.CreatedAt,
                        l.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (language == null)
                {
                    return NotFound("Language not found");
                }

                return Ok(language);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving language with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}