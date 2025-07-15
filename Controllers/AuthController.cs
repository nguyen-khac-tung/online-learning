using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Attributes;
using Online_Learning.Models.DTOs.Request.Auth;
using Online_Learning.Models.DTOs.Response.Auth;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _iAuthService;

        public AuthController(IAuthService authService)
        {
           _iAuthService = authService;
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLogin userLogin)
        {
            string msg = _iAuthService.DoLogin(userLogin, out JwtResponse jwt);
            if(msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<JwtResponse>.SuccessResponse(jwt));
        }

        [HttpPost("Register")]
        public IActionResult Register(UserRegisterRequest request)
        {
            string msg = _iAuthService.RequestRegistrationOtp(request);
            if (msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<string>.SuccessResponse("OTP has been sent to your email."));
        }

        [HttpPost("VerifyRegister")]
        public IActionResult VerifyRegister(VerifyRegisterRequest request)
        {
            string msg = _iAuthService.VerifyRegistration(request, out JwtResponse jwt);
            if (msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<JwtResponse>.SuccessResponse(jwt, "Account created successfully."));
        }

        [HttpPost("ForgotPassword")]
        public IActionResult ForgotPassword(ForgotPasswordRequest request)
        {
            string msg = _iAuthService.RequestForgotPasswordOtp(request);
            if (msg.Length > 0)return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<string>.SuccessResponse("OTP has been sent to your email."));
        }

        [HttpPost("VerifyForgotPassword")]
        public IActionResult VerifyForgotPassword(VerifyForgotPasswordRequest request)
        {
            string msg = _iAuthService.VerifyForgotPassword(request);
            if (msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<string>.SuccessResponse("OTP is valid. You can now change your password."));
        }

        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordRequest request)
        {
            string msg = _iAuthService.ChangePassword(request);
            if (msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<string>.SuccessResponse("Your password has been changed successfully."));
        }

        [HttpPost("SendOtp")]
        public IActionResult SendOtp(SendOtpRequest request)
        {
            string msg = _iAuthService.SendOtp(request.Email);
            if (msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<string>.SuccessResponse("A new OTP has been sent to your email."));
        }

    }
}
