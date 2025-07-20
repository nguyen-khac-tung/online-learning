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
                    .ThenInclude(c => c.Language)
                .Include(c => c.Course)
                    .ThenInclude(c => c.Level)
                .Include(c => c.Course)
                    .ThenInclude(c => c.CourseImages)
                .Include(c => c.Course)
                    .ThenInclude(c => c.CoursePrices)
                .Include(c => c.Course)
                    .ThenInclude(c => c.CourseCategories)
                        .ThenInclude(cc => cc.Category)
                .Where(c => c.UserId == userId)
                .ToList();
        }

        public CartItem? GetCartItemByCartId(long cartItemId)
        {
            return _context.CartItems.Where(c => c.CartItemId == cartItemId).FirstOrDefault();
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
