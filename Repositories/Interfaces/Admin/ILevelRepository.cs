using Online_Learning.Models.Entities;

namespace Online_Learning.Repositories.Interfaces.Admin
{
    public interface ILevelRepository
    {
        Task<IEnumerable<Level>> GetActiveLevelsAsync();
    }
}
