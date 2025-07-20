using Online_Learning.Models.Entities;

namespace Online_Learning.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<Discount>> GetAllAsync();
        Task<Discount?> GetByIdAsync(long id);
        Task<Discount?> GetByCodeAsync(string code);
        Task<Discount> AddAsync(Discount discount);
        Task<Discount> UpdateAsync(Discount discount);
        Task DeleteAsync(Discount discount);
    }
}
