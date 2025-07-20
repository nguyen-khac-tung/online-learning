using Online_Learning.Models.Entities;

namespace Online_Learning.Repositories.Interfaces
{
    public interface ICartRepository
    {
        public CartItem? GetCartItemByCartId(long cartItemId);

        public List<CartItem> GetCartItemsByUserId(string userId);

        public void AddCartItem(CartItem cartItem);

        public void RemoveCartItem(CartItem cartItem);

        public int SaveChanges();
    }
}
