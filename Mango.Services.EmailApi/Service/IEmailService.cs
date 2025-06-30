using Mango.Services.EmailApi.Models.Dto;

namespace Mango.Services.EmailApi.Service
{
    public interface IEmailService
    {
        Task EmailCartLog(CartDto cartDto);
    }
}
