using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;

namespace Online_Learning.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly OnlineLearningContext _context;

        public RoleRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public Role GetRoleById(int roleId)
        {
            return _context.Roles.FirstOrDefault(r => r.RoleId == roleId);
        }
    }
}
