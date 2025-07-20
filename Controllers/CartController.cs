using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        //[HttpGet("GetCartItems")]
        //public IActionResult GetCartItems()
        //{
        //    string msg = _
        //}
    }
}
