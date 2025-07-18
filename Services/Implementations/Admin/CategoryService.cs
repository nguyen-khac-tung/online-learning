using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces.Admin;
using Online_Learning.Services.Interfaces.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Services.Implementations.Admin
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _categoryRepository.GetActiveCategoriesAsync();
        }
    }
} 