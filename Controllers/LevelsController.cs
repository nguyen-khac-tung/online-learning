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
    public class LevelsController : ControllerBase
    {
        private readonly OnlineLearningContext _context;
        private readonly ILogger<LevelsController> _logger;

        public LevelsController(OnlineLearningContext context, ILogger<LevelsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetLevels()
        {
            try
            {
                var levels = await _context.Levels
                    .Where(l => l.DeletedAt == null)
                    .Select(l => new
                    {
                        l.LevelId,
                        l.LevelName,
                        l.Status,
                        l.CreatedAt,
                        l.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(levels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving levels");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetLevel(int id)
        {
            try
            {
                var level = await _context.Levels
                    .Where(l => l.DeletedAt == null && l.LevelId == id)
                    .Select(l => new
                    {
                        l.LevelId,
                        l.LevelName,
                        l.Status,
                        l.CreatedAt,
                        l.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (level == null)
                {
                    return NotFound("Level not found");
                }

                return Ok(level);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving level with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}