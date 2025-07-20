using Microsoft.EntityFrameworkCore;
using Online_Learning.Constants.Enums;
using Online_Learning.Models.DTOs.Request.Admin;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Online_Learning.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {

        private readonly OnlineLearningContext _context;

        public UserRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public string GetUserIdFromClaims(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                   claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<(List<User> users, int totalCount)> GetUsersAsync(UserFilterRequest request)
        {
            var query = _context.Users
                .Include(u => u.Roles)
                .Include(u => u.CourseEnrollments)
                .Where(u => u.Status != (int)UserStatus.Deleted);

            // Apply filters
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    (u.Phone != null && u.Phone.Contains(searchTerm)));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(u => u.Status == (int)request.Status.Value);
            }

            if (request.Role.HasValue)
            {
                query = query.Where(u => u.Roles.Any(r => r.RoleId == (int)request.Role.Value));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = request.SortBy?.ToLower() switch
        {
                "fullname" => request.IsDescending ? query.OrderByDescending(u => u.FullName) : query.OrderBy(u => u.FullName),
                "email" => request.IsDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "createdat" => request.IsDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            // Apply pagination
            var users = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.CourseEnrollments)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Status != (int)UserStatus.Deleted);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email && u.Status != (int)UserStatus.Deleted);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Status = (int)UserStatus.Deleted;
            user.DeletedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == userId && u.Status != (int)UserStatus.Deleted);
        }

        public async Task<bool> EmailExistsAsync(string email, string? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Email == email && u.Status != (int)UserStatus.Deleted);

            if (!string.IsNullOrEmpty(excludeUserId))
            {
                query = query.Where(u => u.UserId != excludeUserId);
        }

            return await query.AnyAsync();
        }

        public async Task<List<Role>> GetUserRolesAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            return user?.Roles.ToList() ?? new List<Role>();
        }

        public async Task<bool> UpdateUserRolesAsync(string userId, List<UserRole> roles)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return false;

            // Clear existing roles
            user.Roles.Clear();

            // Add new roles
            foreach (var roleEnum in roles)
            {
                var role = await _context.Roles.FindAsync((int)roleEnum);
                if (role != null)
        {
                    user.Roles.Add(role);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public User? GetUserById(string userId)
        {
            return _context.Users
                .Where(u => u.UserId == userId && u.Status != (int)UserStatus.Deleted)
                .FirstOrDefault();
        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Email == email && u.Status != (int)UserStatus.Deleted)
                .FirstOrDefault();
        }

        public List<Role> GetRolesByUserId(string userId)
        {
            return _context.Users
                .Where(u => u.UserId == userId && u.Status != (int)UserStatus.Deleted)
                .SelectMany(u => u.Roles)
                .ToList();
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
