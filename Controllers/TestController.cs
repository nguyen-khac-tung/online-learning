using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Models.Entities;

namespace Online_Learning.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		private readonly OnlineLearningContext _context;
        public TestController(OnlineLearningContext context)
        {
            _context = context;
        }

		[HttpGet]
		public IEnumerable<Role> GetRoles()
		{
			return _context.Roles.ToList();
		}
    }
}
