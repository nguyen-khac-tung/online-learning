using Online_Learning.Models.DTOs.Request.Auth;
using Online_Learning.Models.DTOs.Response.Auth;

namespace Online_Learning.Services.Interfaces
{
    public interface IAuthService
    {
        public string DoLogin(UserLogin userLogin, out JwtResponse jwt);
        string RequestRegistrationOtp(UserRegisterRequest request);
        string VerifyRegistration(VerifyRegisterRequest request, out JwtResponse jwt);
        string RequestForgotPasswordOtp(ForgotPasswordRequest request);
        string VerifyForgotPassword(VerifyForgotPasswordRequest request);
        string ChangePassword(ChangePasswordRequest request);
        string SendOtp(string email);
    }
}
