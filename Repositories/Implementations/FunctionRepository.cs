using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;

namespace Online_Learning.Repositories.Implementations
{
    public class FunctionRepository : IFunctionRepository
    {
        private readonly OnlineLearningContext _context;

        public FunctionRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetAllowedApiUrlsForRolesAsync(IEnumerable<string> roleNames)
        {
            var allowedUrls = await _context.Roles
                .Where(r => roleNames.Contains(r.RoleName))
                .SelectMany(r => r.Functions)
                .Select(f => f.ApiUrl.ToLower())
                .Distinct()
                .ToListAsync();

            return allowedUrls;
        }
    }
}
