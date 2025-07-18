using Online_Learning.Models.Entities;

namespace Online_Learning.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User? GetUserById(string userId);
        public User? GetUserByEmail(string email);
        public List<Role> GetRolesByUserId(string userId);

        public void AddUser(User user);

        public void UpdateUser(User user);

        int SaveChanges();
    }
}
