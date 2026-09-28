using AutoMapper;
using Azure.Core;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services
{
    public class AuthService(
        AppDbContext _db,
        UserManager<ApplicationUser> _userManager,
        IMapper _mapper,
        IJwtTokenGenerator _tokenGenerator,
        RoleManager<IdentityRole> _roleManager) : IAuthService
    {
        public async Task<bool> AssignRole(string UserId, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == UserId);
            if (user != null) 
            { 
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }

                await _userManager.AddToRoleAsync(user, roleName);

                return true;
            }

            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto request)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email!.ToLower() == request.Email.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user!, request.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }

            var token = _tokenGenerator.GenerateToken(user!);

            UserDto userDto = new()
            {
                Email = user.Email!,
                //Id = user.Id,
                Name = user.UserName!,
                //PhoneNumber = user.PhoneNumber!
            };

            LoginResponseDto responseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token
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
