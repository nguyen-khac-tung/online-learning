using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        //public  Get
    }
}
