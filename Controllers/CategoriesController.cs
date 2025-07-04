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
    public class CategoriesController : ControllerBase
    {
        private readonly OnlineLearningContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(OnlineLearningContext context, ILogger<CategoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.DeletedAt == null)
                    .Select(c => new
                    {
                        c.CategoryId,
                        c.CategoryName,
                        c.Status,
                        c.CreatedAt,
                        c.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Categories/course-categories
        [HttpGet("course-categories")]
        public async Task<ActionResult<IEnumerable<object>>> GetCourseCategories([FromQuery] string courseId)
        {
            var categories = await _context.CourseCategories
                .Where(cc => cc.CourseId == courseId)
                .Include(cc => cc.Category)
                .Where(cc => cc.Category.DeletedAt == null)
                .Select(cc => new
                {
                    cc.CategoryId,
                    cc.Category.CategoryName
                })
                .ToListAsync();
            return Ok(categories);
        }
    }
}