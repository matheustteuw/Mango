using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Services.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegisterRequestDto request);
        Task<LoginResponseDto> Login(LoginResponseDto request);
    }
}
