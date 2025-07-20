using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Attributes;
using Online_Learning.Models.DTOs.Request.Auth;
using Online_Learning.Models.DTOs.Response.Common;
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

        [DynamicAuthorize]
        [HttpPost("Test")]
        public IActionResult Test(UserLogin userLogin)
        {
            return Ok(ApiResponse<string>.SuccessResponse("mess"));
        }


        //Man phan quyen

        //Logout

        //Man Profile


        //Login/Register google

        //Profle Image
    }
}
