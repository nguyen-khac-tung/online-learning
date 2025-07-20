using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;

namespace Online_Learning.Repositories.Implementations
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly OnlineLearningContext _context;
		public CategoryRepository(OnlineLearningContext context)
		{
			_context = context;
		}
		public async Task<IEnumerable<dynamic>> GetAllCategoriesAsync()
		{
			return await _context.Categories
				.Where(c => c.Status == 1)
				.Select(c => new
				{
					c.CategoryId,
					c.CategoryName
				})
				.ToListAsync();
		}
	}
}
