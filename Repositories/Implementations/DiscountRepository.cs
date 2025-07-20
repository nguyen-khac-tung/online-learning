using Microsoft.EntityFrameworkCore;
using Online_Learning.Models;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;

namespace Online_Learning.Repositories.Implementations
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly OnlineLearningContext _context;

        public DiscountRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Discount>> GetAllAsync() =>
            await _context.Discounts.OrderByDescending(d => d.CreatedAt).ToListAsync();

        public async Task<Discount?> GetByIdAsync(long id) =>
            await _context.Discounts.FindAsync(id);

        public async Task<Discount?> GetByCodeAsync(string code) =>
            await _context.Discounts.FirstOrDefaultAsync(d => d.Code == code.ToUpper());

        public async Task<Discount> AddAsync(Discount discount)
        {
            discount.DiscountId = new Random().Next(10000, 99999);
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount> UpdateAsync(Discount discount)
        {
            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task DeleteAsync(Discount discount)
        {
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
        }
    }
}