using Mango.Services.AuthApi.Models.Dto;

namespace Mango.Services.AuthApi.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto requestDto);
        Task<LoginResponseDto> Login(LoginRequestDto requestDto);

        Task<bool> AssignRole(string email,string roleName);

    }
}
