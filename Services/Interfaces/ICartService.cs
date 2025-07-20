using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.Entities;
using System.Security.Claims;

namespace Online_Learning.Services.Interfaces
{
    public interface ICartService
    {
        string GetUserCartItems(ClaimsPrincipal currentUser, out List<CartItemsDto> listCartItemDto);
        string AddCartItem(CartRequestDto cartRequestDto, ClaimsPrincipal currentUser, out CartItemsDto cartItemDto);
        string DeleteCartItem(long cartItemId);
    }
}
