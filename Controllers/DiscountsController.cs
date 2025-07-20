using Microsoft.AspNetCore.Mvc;
using Online_Learning.Models.DTOs.Discount;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountService _service;

        public DiscountsController(IDiscountService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DiscountRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? date,
            [FromQuery] int? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            try
            {
                var result = await _service.GetValidDiscountsAsync(date, status, page, pageSize, search);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/usage")]
        public async Task<IActionResult> GetUsage(long id)
        {
            try
            {
                var info = await _service.GetUsageInfoAsync(id);
                return Ok(info);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] DiscountUpdateRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _service.SoftDeleteAsync(id);
                return Ok(new { message = "Discount deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            try
            {
                var result = await _service.ToggleStatusAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}