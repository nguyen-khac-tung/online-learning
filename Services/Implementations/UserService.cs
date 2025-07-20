using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Online_Learning.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public string GetUserProfile(ClaimsPrincipal currentUser, out UserProfileDto userProfile)
        {
            userProfile = null;

            var userId = _userRepository.GetUserIdFromClaims(currentUser);

            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return "User not found.";
            }

            var roles = _userRepository.GetRolesByUserId(userId);
            userProfile = new UserProfileDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                DoB = user.DoB,
                Gender = user.Gender,
                Phone = user.Phone,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
                Roles = roles.Select(r => r.RoleName).ToList()
            };

            return "";
        }

        public string UpdateUserProfile(ClaimsPrincipal currentUser, UpdateProfileRequestDto request)
        {
            var userId = _userRepository.GetUserIdFromClaims(currentUser);

            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return "User not found.";
            }

            user.FullName = request.FullName;
            user.DoB = request.DoB;
            user.Gender = request.Gender;
            user.Phone = request.Phone;
            user.Address = request.Address;
            user.AvatarUrl = request.AvatarUrl;
            user.UpdatedAt = DateTime.Now;

            _userRepository.UpdateUser(user);
            _userRepository.SaveChanges();

            return "";
        }

    }
}
