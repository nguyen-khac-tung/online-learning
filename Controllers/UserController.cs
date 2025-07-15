using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Attributes;
using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [DynamicAuthorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("Profile")]
        public IActionResult GetUserProfile()
        {
            string msg = _userService.GetUserProfile(User, out UserProfileDto userProfile);
            if (msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(userProfile));
        }

        [HttpPut("UpdateProfile")]
        public IActionResult UpdateUserProfile(UpdateProfileRequestDto request)
        {
            string msg = _userService.UpdateUserProfile(User, request);
            if (msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<string>.SuccessResponse("Profile updated successfully."));
        }
    }
}
