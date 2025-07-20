using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Online_Learning.Services;
using Online_Learning.Services.Interfaces;
using Online_Learning.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;

namespace Online_Learning.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class LevelsController : ControllerBase
    {
        private readonly ILevelService _levelService;
        private readonly ILogger<LevelsController> _logger;

        public LevelsController(ILevelService levelService, ILogger<LevelsController> logger)
        {
            _levelService = levelService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetLevels()
        {
            try
            {
                var levels = await _levelService.GetActiveLevelsAsync();
                var result = levels.Select(l => new
                {
                    l.LevelId,
                    l.LevelName,
                    l.Status,
                    l.CreatedAt,
                    l.UpdatedAt
                }).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving levels");
                return StatusCode(500, "Internal server error");
            }
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<object>> GetLevel(int id)
        //{
        //    try
        //    {
        //        var level = await _context.Levels
        //            .Where(l => l.DeletedAt == null && l.LevelId == id)
        //            .Select(l => new
        //            {
        //                l.LevelId,
        //                l.LevelName,
        //                l.Status,
        //                l.CreatedAt,
        //                l.UpdatedAt
        //            })
        //            .FirstOrDefaultAsync();

        //        if (level == null)
        //        {
        //            return NotFound("Level not found");
        //        }

        //        return Ok(level);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving level with ID {Id}", id);
        //        return StatusCode(500, "Internal server error");
        //    }
        //}
    }
}