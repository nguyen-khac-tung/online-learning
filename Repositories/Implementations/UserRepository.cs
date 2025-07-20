using Microsoft.EntityFrameworkCore;
using Online_Learning.Constants.Enums;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
