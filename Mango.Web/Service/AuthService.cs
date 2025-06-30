using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using Mango.Web.Utlility;
using static Mango.Web.Utlility.SD;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Post,
                Data = registrationRequestDto,
                Url = SD.AuthApiBase + "api/auth/AssignRole"
            },isBearer:false);
        }

        public async Task<ResponseDto> LoginAsync(LoginRequestDto login)
        {
           var result =  await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Post,
                Data = login,
                Url = SD.AuthApiBase + "api/auth/login"
            },isBearer:false);

            return result;
        }

        public async Task<ResponseDto> RegistrationAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Post,
                Data = registrationRequestDto,
                Url = SD.AuthApiBase + "api/auth/register"
            }, isBearer: false);
        }

    }
}
