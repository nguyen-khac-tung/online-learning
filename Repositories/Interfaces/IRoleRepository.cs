using Online_Learning.Models.Entities;

namespace Online_Learning.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Role GetRoleById(int roleId);
    }
}
