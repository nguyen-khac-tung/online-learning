using Online_Learning.Models.Entities;
using System.Security.Claims;

namespace Online_Learning.Repositories.Interfaces
{
    public interface IUserRepository
    {
        string GetUserIdFromClaims(ClaimsPrincipal claimsPrincipal);

        User? GetUserById(string userId);
        public User? GetUserByEmail(string email);
        public List<Role> GetRolesByUserId(string userId);

        public void AddUser(User user);

        public void UpdateUser(User user);

        int SaveChanges();
    }
}
