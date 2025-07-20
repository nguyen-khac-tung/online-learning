using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Attributes;
using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.Entities;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [DynamicAuthorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("GetCartItems")]
        public IActionResult GetCartItems()
        {
            string msg = _cartService.GetUserCartItems(User, out List<CartItemsDto> listCartItemDto);
            if(msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<object>.SuccessResponse(listCartItemDto));
        }

        [HttpPost("AddCartItem")]
        public IActionResult AddCartItem(CartRequestDto cartRequestDto)
        {
            string msg = _cartService.AddCartItem(cartRequestDto, User, out CartItemsDto cartItemDto);
            if (msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<object>.SuccessResponse(cartItemDto));
        }

        [HttpDelete("DeleteCartItem/{cartItemId}")]
        public IActionResult DeleteCartItem(long cartItemId)
        {
            string msg = _cartService.DeleteCartItem(cartItemId);
            if (msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<object>.SuccessResponse("", "Cart item has been deleted"));
        }
    }
}
