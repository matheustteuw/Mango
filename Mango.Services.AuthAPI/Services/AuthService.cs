using AutoMapper;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public AuthService(
            AppDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _db = db; 
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto request)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName!.ToLower() == request.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user!, request.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }

            UserDto userDto = new()
            {
                Email = user.Email!,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber!
            };

            LoginResponseDto responseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = ""
            };

            return responseDto;
        }

        public async Task<string> Register(RegisterRequestDto request)
        {
            ApplicationUser user = _mapper.Map<ApplicationUser>(request);

            try
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == request.Email);

                    UserDto userDto = _mapper.Map<UserDto>(userToReturn);

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault()!.Description;
                }
            }
            catch (Exception) { }

            return "Error Encountered";
        }
    }
}
