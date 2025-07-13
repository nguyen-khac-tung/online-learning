namespace Online_Learning.Repositories.Interfaces
{
	public interface ILesssonRepository
	{
		Task<List<long>> GetLessonIdCompletedAsync(string userId);
	}
}
