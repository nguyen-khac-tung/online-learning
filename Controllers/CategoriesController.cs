using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Repositories.Interfaces;

namespace Online_Learning.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly ICategoryRepository _categoryRepository;
		public CategoriesController(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}

        /// <summary>
        /// Get category for filter 
        /// </summary>
        /// <remarks>Author: HaiPDHE172178 | Role: Guest</remarks>
        [HttpGet]
		public async Task<IActionResult> GetAllCategoryAsync()
		{
			var result = await _categoryRepository.GetAllCategoriesAsync();
			return Ok(ApiResponse<dynamic>.SuccessResponse(result, "Categories retrieved successfully"));
		}
	}
}
