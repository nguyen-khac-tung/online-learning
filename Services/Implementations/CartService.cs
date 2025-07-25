using Online_Learning.Constants;
using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Interfaces;
using System.Security.Claims;

namespace Online_Learning.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;

        public CartService(ICartRepository cartRepository, ICourseRepository courseRepository, IUserRepository userRepository)
        {
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;

        }

        public string GetUserCartItems(ClaimsPrincipal currentUser, out List<CartItemsDto> listCartItemDto)
        {
            var userId = _userRepository.GetUserIdFromClaims(currentUser);

            List<CartItem> listItem = _cartRepository.GetCartItemsByUserId(userId);

            listCartItemDto = MapToCartItemDto(listItem);
            return "";
        }

        public string AddCartItem(CartRequestDto cartRequestDto, ClaimsPrincipal currentUser, out CartItemsDto cartItemDto)
        {
            cartItemDto = null;

            var course = _courseRepository.GetCourseById(cartRequestDto.CourseId);
            if (course == null) return Messages.CourseNotFoundCart;   

            var userId = _userRepository.GetUserIdFromClaims(currentUser);

            var cartItem = new CartItem
            {
                UserId = userId,
                CourseId = course.CourseId,
                CreatedAt = DateTime.Now,
            };

            _cartRepository.AddCartItem(cartItem);
            _cartRepository.SaveChanges();

            cartItemDto = new CartItemsDto
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                CourseImgUrl = course.CourseImages
                                .OrderByDescending(i => i.ImageId)
                                .Select(i => i.ImageUrl)
                                .FirstOrDefault(),
                StudyTime = course.StudyTime,
                LevelName = course.Level.LevelName,
                Price = course.CoursePrices
                        .OrderByDescending(cp => cp.CreateAt)
                        .Select(cp => cp.Price)
                        .FirstOrDefault()
            };
            return "";
        }

        public string DeleteCartItem(long cartItemId)
        {
            var cartItem = _cartRepository.GetCartItemByCartId(cartItemId);
            if (cartItem == null) return Messages.CartItemNotFound;

            _cartRepository.RemoveCartItem(cartItem);
            _cartRepository.SaveChanges();

            return "";
        }

        private List<CartItemsDto> MapToCartItemDto(List<CartItem> listItem)
        {
            var listCartItemDto = new List<CartItemsDto>();

            if (listItem == null || listItem.Count() == 0)
                return listCartItemDto;

            listCartItemDto = listItem.Select(c => new CartItemsDto
            {
                CartItemId = c.CartItemId,
                CourseId = c.CourseId,

                CourseName = c.Course.CourseName,

                CourseImgUrl = c.Course.CourseImages
                                .OrderByDescending(i => i.ImageId)
                                .Select(i => i.ImageUrl)
                                .FirstOrDefault(),

                Category = c.Course.CourseCategories
                            .Select(ct => ct.Category.CategoryName)
                            .ToList(),

                StudyTime = c.Course.StudyTime,


                LevelName = c.Course.Level.LevelName,

                Language = c.Course.Language.LanguageName,

                Price = c.Course.CoursePrices
                        .OrderByDescending(cp => cp.CreateAt)
                        .Select(cp => cp.Price)
                        .FirstOrDefault()
            })
            .ToList();

            return listCartItemDto;
        }
    }
}
