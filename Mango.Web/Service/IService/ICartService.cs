
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto> GetCartList(string userId);
        Task<ResponseDto> UpsertItem(CartDto cartDto);
        Task<ResponseDto> ApplyRemoveCoupon(CartDto cartDto);
        Task<ResponseDto> RemoveItem(string cartDetailsId);

        Task<ResponseDto> EmailCart(CartDto cartDto);

    }
}
