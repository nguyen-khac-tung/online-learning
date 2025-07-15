namespace Online_Learning.Repositories.Interfaces
{
    public interface IFunctionRepository
    {
        Task<List<string>> GetAllowedApiUrlsForRolesAsync(IEnumerable<string> roleNames);
    }
}
