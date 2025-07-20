using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Learning.Models;
using Online_Learning.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Online_Learning.Services;
using Online_Learning.Services.Interfaces;
using Online_Learning.Services.Interfaces.Admin;

namespace Online_Learning.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetActiveCategoriesAsync();
                var result = categories.Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.Status,
                    c.CreatedAt,
                    c.UpdatedAt
                }).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, "Internal server error");
            }
        }

        //// GET: api/Categories/course-categories
        //[HttpGet("course-categories")]
        //public async Task<ActionResult<IEnumerable<object>>> GetCourseCategories([FromQuery] string courseId)
        //{
        //    var categories = await _context.CourseCategories
        //        .Where(cc => cc.CourseId == courseId)
        //        .Include(cc => cc.Category)
        //        .Where(cc => cc.Category.DeletedAt == null)
        //        .Select(cc => new
        //        {
        //            cc.CategoryId,
        //            cc.Category.CategoryName
        //        })
        //        .ToListAsync();
        //    return Ok(categories);
        //}
    }
}