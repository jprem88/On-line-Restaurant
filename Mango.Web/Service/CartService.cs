
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utlility;
using static Mango.Web.Utlility.SD;


namespace Mango.Web.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto> ApplyRemoveCoupon(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType =ApiType.Post,
                Data = cartDto,
                Url = $"{SD.ShopingCartApiBase}api/cart/ApplyOrRemoveCoupon"

            });
        }

        public async Task<ResponseDto> GetCartList(string userId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.Get,
                Url = $"{SD.ShopingCartApiBase}api/cart/GetCart/{userId}"

            });
        }

        public async Task<ResponseDto> RemoveItem(string cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.Post,
                Data = cartDetailsId,
                Url = $"{SD.ShopingCartApiBase}api/cart/RemoveItem"
            });
        }

        public async Task<ResponseDto> UpsertItem(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.Post,
                Data = cartDto,
                Url = $"{SD.ShopingCartApiBase}api/cart"
            });
        }

      public async  Task<ResponseDto> EmailCart(CartDto cartDto) 
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.Post,
                Data = cartDto,
                Url = $"{SD.ShopingCartApiBase}api/cart/EmailCartRequest"

            });
        }
    }
}
