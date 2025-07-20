using Microsoft.EntityFrameworkCore;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;

namespace Online_Learning.Repositories.Implementations
{
    public class CartRepository: ICartRepository
    {
        private readonly OnlineLearningContext _context;

        public CartRepository(OnlineLearningContext context)
        {
            _context = context;
        }

        public List<CartItem> GetCartItemsByUserId(string userId)
        {
            return _context.CartItems
                .Include(c => c.User)
                .Include(c => c.Course)
                .Where(c => c.UserId == userId)
                .ToList();
        }

        public void AddCartItem(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
        }

        public void RemoveCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
