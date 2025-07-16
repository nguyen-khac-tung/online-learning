using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Repositories.Interfaces.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Learning.Repositories.Implementations.Admin
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly OnlineLearningContext _context;
        public LanguageRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Language>> GetActiveLanguagesAsync()
        {
            return await _context.Languages
                .Where(l => l.DeletedAt == null)
                .ToListAsync();
        }
    }
} 