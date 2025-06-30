using Mango.Services.AuthApi.Data;
using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Models.Dto;
using Mango.Services.AuthApi.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthApi.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenratorcs _jwtTokenGenratorcs;
        public AuthService(AppDbContext appDbContext, 
            UserManager<ApplicationUser> userManager,
             RoleManager<IdentityRole> roleManager,
              IJwtTokenGenratorcs jwtTokenGenratorcs)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenratorcs = jwtTokenGenratorcs;
        }

        public async Task<bool> AssignRole(string email, string roleName) 
        {
            var user = _appDbContext.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == email.ToLower());
            if(user !=null)
            {
                if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //////// create role if role is not exits
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto requestDto)
        {
            try
            {
                var user = _appDbContext.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == requestDto.UserName.ToLower());
                var isValid = await _userManager.CheckPasswordAsync(user, requestDto.Password);
                if (user == null || !isValid)
                {
                    return new LoginResponseDto()
                    {
                        User = null,
                        Token = ""
                    };
                }

                UserDto userDto = new()
                {
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Id = user.Id
                };

                var roles = await _userManager.GetRolesAsync(user);
                LoginResponseDto loginResponseDto = new()
                {
                    User = userDto,
                    Token = _jwtTokenGenratorcs.GenrateToken(user, roles)
                };
                return loginResponseDto;
            }
            catch (Exception)
            {

                
            }
            return new LoginResponseDto()
            {
                User = null,
                Token = ""
            };
        }

        public async Task<string> Register(RegistrationRequestDto requestDto)
        {
            ApplicationUser user = new()
            {
                UserName = requestDto.Email,
                Email = requestDto.Email,
                NormalizedEmail = requestDto.Email.ToUpper(),
                Name = requestDto.Name,
                PhoneNumber = requestDto.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(user, requestDto.Password);
                if(result.Succeeded)
                {
                    var createdUser = _appDbContext.ApplicationUsers.FirstOrDefault(x => x.UserName == requestDto.Email);
                    UserDto userDto = new()
                    {
                        Email = createdUser.Email,
                        Name = createdUser.Name,
                        Id = createdUser.Id,
                        PhoneNumber = createdUser.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }

            catch(Exception ex) {
                //throw new Exception();
            }
            return "Error encountered";
            
           

        }

        public async Task<string> RegisterWithRole(RegistrationRequestDto requestDto)
        {
            ApplicationUser user = new()
            {
                UserName = requestDto.Email,
                Email = requestDto.Email,
                NormalizedEmail = requestDto.Email.ToUpper(),
                Name = requestDto.Name,
                PhoneNumber = requestDto.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(user, requestDto.Password);
                if (result.Succeeded)
                {
                    var createdUser = _appDbContext.ApplicationUsers.FirstOrDefault(x => x.UserName == requestDto.Email);
                    UserDto userDto = new()
                    {
                        Email = createdUser.Email,
                        Name = createdUser.Name,
                        Id = createdUser.Id,
                        PhoneNumber = createdUser.PhoneNumber
                    };

                    await AssignRole(requestDto.Email, requestDto.Role);



                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }

            catch (Exception ex)
            {
                //throw new Exception();
            }
            return "Error encountered";



        }
    }
}
