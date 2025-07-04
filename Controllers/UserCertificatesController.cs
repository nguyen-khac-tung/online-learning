using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;

namespace Online_Learning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserCertificatesController : ControllerBase
    {
        private readonly OnlineLearningContext _context;
        private readonly ILogger<UserCertificatesController> _logger;

        public UserCertificatesController(OnlineLearningContext context, ILogger<UserCertificatesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUserCertificates()
        {
            try
            {
                var certificates = await _context.UserCertificates
                    .Include(uc => uc.Course)
                    .Where(uc => uc.Course != null && uc.Course.DeletedAt == null)
                    .Select(uc => new
                    {
                        uc.CertificateId,
                        uc.UserId,
                        uc.CourseId,
                        uc.CreatedAt,
                        Course = uc.Course != null ? new { uc.Course.CourseId, uc.Course.CourseName } : null
                    })
                    .ToListAsync();

                return Ok(certificates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user certificates");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
