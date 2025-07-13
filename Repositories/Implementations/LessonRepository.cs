using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;

namespace Online_Learning.Repositories.Implementations
{
	public class LessonRepository : ILesssonRepository
	{
		private readonly OnlineLearningContext _context;

		public LessonRepository(OnlineLearningContext context)
		{
			_context = context;
		}
		public async Task<List<long>> GetLessonIdCompletedAsync(string userId)
		{
			return await _context.LessonProgresses
								.Where(lg => lg.UserId == userId)
								.Select(lg=> lg.LessonId)
								.ToListAsync();
		}
	}
}
