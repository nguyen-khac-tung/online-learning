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
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;
        private readonly ILogger<LanguagesController> _logger;

        public LanguagesController(ILanguageService languageService, ILogger<LanguagesController> logger)
        {
            _languageService = languageService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetLanguages()
        {
            try
            {
                var languages = await _languageService.GetActiveLanguagesAsync();
                var result = languages.Select(l => new
                {
                    l.LanguageId,
                    l.LanguageName,
                    l.Status,
                    l.CreatedAt,
                    l.UpdatedAt
                }).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving languages");
                return StatusCode(500, "Internal server error");
            }
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<object>> GetLanguage(int id)
        //{
        //    try
        //    {
        //        var language = await _context.Languages
        //            .Where(l => l.DeletedAt == null && l.LanguageId == id)
        //            .Select(l => new
        //            {
        //                l.LanguageId,
        //                l.LanguageName,
        //                l.Status,
        //                l.CreatedAt,
        //                l.UpdatedAt
        //            })
        //            .FirstOrDefaultAsync();

        //        if (language == null)
        //        {
        //            return NotFound("Language not found");
        //        }

        //        return Ok(language);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving language with ID {Id}", id);
        //        return StatusCode(500, "Internal server error");
        //    }
        //}
    }
}