using Online_Learning.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Services.Interfaces.Admin
{
    public interface ILevelService
    {
        Task<IEnumerable<Level>> GetActiveLevelsAsync();
    }
} 