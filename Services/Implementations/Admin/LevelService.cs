using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces.Admin;
using Online_Learning.Services.Interfaces.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Services.Implementations.Admin
{
    public class LevelService : ILevelService
    {
        private readonly ILevelRepository _levelRepository;
        public LevelService(ILevelRepository levelRepository)
        {
            _levelRepository = levelRepository;
        }

        public async Task<IEnumerable<Level>> GetActiveLevelsAsync()
        {
            return await _levelRepository.GetActiveLevelsAsync();
        }
    }
} 