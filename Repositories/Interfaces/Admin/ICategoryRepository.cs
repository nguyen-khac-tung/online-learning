using Online_Learning.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Repositories.Interfaces.Admin
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    }
} 