using Online_Learning.Models.DTOs.Response.User;

namespace Online_Learning.Models.DTOs.Response.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; }

        public UserDto User { get; set; }

        public AuthResponseDto() { }
    }
}
