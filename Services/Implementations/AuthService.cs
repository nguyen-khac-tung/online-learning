using Azure.Core;
using Microsoft.IdentityModel.Tokens;
using Online_Learning.Constants.Enums;
using Online_Learning.Models.DTOs.Request.Auth;
using Online_Learning.Models.DTOs.Response.Auth;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Implementations;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Online_Learning.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _iConfig;
        private readonly IUserRepository _iUserRepository;
        private readonly IUserOtpRepository _iUserOtpRepository;
        private readonly IRoleRepository _iRoleRepository;
        private readonly IEmailService _iEmailService;

        public AuthService(IConfiguration configuration,
                           IUserRepository userRepository,
                           IUserOtpRepository userOtpRepository,
                           IRoleRepository roleRepository,
                           IEmailService emailService)
        {
            _iConfig = configuration;
            _iUserRepository = userRepository;
            _iUserOtpRepository = userOtpRepository;
            _iRoleRepository = roleRepository;
            _iEmailService = emailService;
        }

        public string DoLogin(UserLogin userLogin, out AuthResponseDto auth)
        {
            auth = new AuthResponseDto();

            var user = _iUserRepository.GetUserByEmail(userLogin.Email);
            if (user == null) return "Username or password is incorrect";
            if (user.Status == (int)UserStatus.Inactive)
                return "Account has been locked";

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password);
            if (!isPasswordCorrect) return "Username or password is incorrect";

            auth.Token = GenerateJwtToken(user);
            auth.User = new UserDto()
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Roles = user.Roles.Select(u => u.RoleName).ToList(),
                AvatarUrl = user.AvatarUrl
            };

            return "";
        }

        public string RequestRegistrationOtp(UserRegisterRequest request)
        {
            var user = _iUserRepository.GetUserByEmail(request.Email);
            if (user != null) return "Email has already been registered.";

            var msg = SendOtp(request.Email);
            if (msg.Length > 0) return msg;

            return "";
        }

        public string VerifyRegistration(VerifyRegisterRequest request, out AuthResponseDto auth)
        {
            auth = new AuthResponseDto();

            var msg = VerfifyOtpCode(request.Email, request.OtpCode, out UserOtp userOtp);
            if(msg.Length > 0) return msg;

            var newUser = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Status = (int)UserStatus.Active,
                CreatedAt = DateTime.Now,
            };

            var studentRole = _iRoleRepository.GetRoleById((int)UserRole.Student);
            if (studentRole == null) return "System configuration error: Student role not found.";
            newUser.Roles.Add(studentRole);

            _iUserRepository.AddUser(newUser);

            userOtp.UsedAt = DateTime.Now;
            _iUserOtpRepository.UpdateOtp(userOtp);

            _iUserRepository.SaveChanges();

            auth.Token = GenerateJwtToken(newUser);
            auth.User = new UserDto()
            {
                UserId = newUser.UserId,
                Email = newUser.Email,
                FullName = newUser.FullName,
                Roles = newUser.Roles.Select(u => u.RoleName).ToList(),
                AvatarUrl = newUser.AvatarUrl
            };

            return "";
        }

        public string RequestForgotPasswordOtp(ForgotPasswordRequest request)
        {
            var user = _iUserRepository.GetUserByEmail(request.Email);
            if (user == null) return "Email address doesn't exists in our system";

            var msg = SendOtp(request.Email);
            if(msg.Length > 0) return msg;

            return "";
        }

        public string VerifyForgotPassword(VerifyForgotPasswordRequest request)
        {
            var msg = VerfifyOtpCode(request.Email, request.OtpCode, out UserOtp userOtp);
            if (msg.Length > 0) return msg;

            return "";
        }

        public string ChangePassword(ChangePasswordRequest request)
        {
            var user = _iUserRepository.GetUserByEmail(request.Email);
            if (user == null) return "User not found.";

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _iUserRepository.UpdateUser(user);

            _iUserRepository.SaveChanges();
            return "";
        }

        public string SendOtp(string email)
        {
            try
            {
                string otp = ProcessAndSaveOtp(email);
                SendOtpByEmail(email, otp);
            }
            catch (Exception) { return "An error occurred while sending the OTP. Please try again."; }

            return "";
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_iConfig["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var userRole = _iUserRepository.GetRolesByUserId(user.UserId);
            foreach (var role in userRole)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
            }

            var token = new JwtSecurityToken(
                issuer: _iConfig["Jwt:Issuer"],
                audience: _iConfig["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_iConfig["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string VerfifyOtpCode(string email, string otpCodeRequested, out UserOtp userOtp)
        {
            userOtp = _iUserOtpRepository.GetOtpByEmail(email);
            if (userOtp == null || userOtp.OtpCode != otpCodeRequested) return "Invalid OTP code.";
            if (userOtp.ExpiresAt < DateTime.Now) return "OTP code has expired.";
            if (userOtp.UsedAt != null) return "OTP code has already been used.";

            return "";
        }

        private string ProcessAndSaveOtp(string email)
        {
            string otpCode = new Random().Next(100000, 999999).ToString();
            var otpExpires = DateTime.Now.AddMinutes(5);

            var userOtp = _iUserOtpRepository.GetOtpByEmail(email);

            if (userOtp != null)
            {
                userOtp.OtpCode = otpCode;
                userOtp.ExpiresAt = otpExpires;
                userOtp.UsedAt = null;
                _iUserOtpRepository.UpdateOtp(userOtp);
            }

            if (userOtp == null)
            {
                userOtp = new UserOtp
                {
                    Email = email,
                    OtpCode = otpCode,
                    ExpiresAt = otpExpires,
                    CreatedAt = DateTime.Now
                };
                _iUserOtpRepository.CreateOtp(userOtp);
            }

            _iUserOtpRepository.SaveChanges();
            return otpCode;
        }

        private void SendOtpByEmail(string toEmail, string otpCode)
        {
            string emailBody = $"Your verification code is: <h2>{otpCode}</h2>This code will expire in 5 minutes.";
            _iEmailService.SendMail(toEmail, "Account Verification OTP", emailBody);
        }
    }
}
