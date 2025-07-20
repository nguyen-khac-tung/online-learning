using Online_Learning.Constants.Enums;
using Online_Learning.Models.DTOs.Request.Admin;
using Online_Learning.Models.Entities;

namespace Online_Learning.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<(List<User> users, int totalCount)> GetUsersAsync(UserFilterRequest request);
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> UserExistsAsync(string userId);
        Task<bool> EmailExistsAsync(string email, string? excludeUserId = null);
        Task<List<Role>> GetUserRolesAsync(string userId);
        Task<bool> UpdateUserRolesAsync(string userId, List<UserRole> roles);
        User? GetUserById(string userId);
        public User? GetUserByEmail(string email);
        public List<Role> GetRolesByUserId(string userId);

        public void AddUser(User user);

        public void UpdateUser(User user);

        int SaveChanges();
    }
}
