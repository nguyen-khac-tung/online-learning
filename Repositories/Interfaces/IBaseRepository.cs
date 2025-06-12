namespace Online_Learning.Repositories.Interfaces
{
	public interface IBaseRepository<T> where T : class
	{
		Task<T> CreateAsync(T entity);
		Task<T> GetByIdAsync(int id);
		Task<IEnumerable<T>> GetAllAsync();
		Task UpdateAsync(T entity);
		Task DeleteAsync(T entity);
	}
}
