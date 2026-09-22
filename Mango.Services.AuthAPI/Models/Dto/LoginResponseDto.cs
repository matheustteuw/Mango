namespace Mango.Services.AuthAPI.Models.Dto
{
    public class LoginResponseDto
    {
        public UserDto? User { get; set; }
        public string UserName { get; set; } = string.Empty; 
        public string Password { get; set; } = string.Empty; 
        public string Token { get; set; } = string.Empty;
    }
}
