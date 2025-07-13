namespace Online_Learning.Repositories.Interfaces
{
	public interface ICategoryRepository
	{
		Task<IEnumerable<dynamic>> GetAllCategoriesAsync();
	}
}
