using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.User;
using System.Security.Claims;

namespace Online_Learning.Services.Interfaces
{
    public interface IUserService
    {
        string GetUserProfile(ClaimsPrincipal currentUser, out UserProfileDto userProfile);
        string UpdateUserProfile(ClaimsPrincipal currentUser, UpdateProfileRequestDto request);
    }
}
