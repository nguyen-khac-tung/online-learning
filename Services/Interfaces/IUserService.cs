using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.User;
using System.Security.Claims;
using Online_Learning.Models.DTOs.Request.Admin;
using Online_Learning.Models.DTOs.Response.Admin;

namespace Online_Learning.Services.Interfaces
{
    public interface IUserService
    {
        Task<AdminApiResponse<PagedResponse<UserResponse>>> GetUsersAsync(UserFilterRequest request);
        Task<AdminApiResponse<UserResponse>> GetUserByIdAsync(string userId);
        Task<AdminApiResponse<UserResponse>> CreateUserAsync(CreateUserRequest request);
        Task<AdminApiResponse<UserResponse>> UpdateUserAsync(string userId, UpdateUserRequest request);
        Task<AdminApiResponse<bool>> DeleteUserAsync(string userId);
        Task<AdminApiResponse<bool>> ToggleUserStatusAsync(string userId);
        Task<AdminApiResponse<bool>> ResetPasswordAsync(string userId);
        Task<AdminApiResponse<bool>> AssignRoleAsync(string userId, AssignRoleRequest request);
        Task<byte[]> ExportUsersToExcelAsync(UserFilterRequest request);
        Task<byte[]> ExportUsersToPdfAsync(UserFilterRequest request);
        string GetUserProfile(ClaimsPrincipal currentUser, out UserProfileDto userProfile);
        string UpdateUserProfile(ClaimsPrincipal currentUser, UpdateProfileRequestDto request);
    }
}
