namespace Online_Learning.Repositories.Interfaces
{
	public interface IBaseRepository<T> where T : class
	{
		Task<T?> GetByIdAsync(string? id);
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(T entity);
		Task<T> GetByIdAsync(params object[] keyValues);
	}
}
