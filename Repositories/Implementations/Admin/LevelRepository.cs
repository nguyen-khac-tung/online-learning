using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Repositories.Interfaces.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Learning.Repositories.Implementations.Admin
{
    public class LevelRepository : ILevelRepository
    {
        private readonly OnlineLearningContext _context;
        public LevelRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Level>> GetActiveLevelsAsync()
        {
            return await _context.Levels
                .Where(l => l.DeletedAt == null)
                .ToListAsync();
        }
    }
} 