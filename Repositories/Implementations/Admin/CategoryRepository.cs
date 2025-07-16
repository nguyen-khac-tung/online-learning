using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Learning.Repositories.Implementations.Admin
{
    public class CategoryRepository :ICategoryRepository
    {
        private readonly OnlineLearningContext _context;
        public CategoryRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.DeletedAt == null)
                .ToListAsync();
        }
    }
} 