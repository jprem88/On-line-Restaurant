namespace Mango.Services.ShopingCartApi.Models.Dto
{
    public class CartDto
    {
        public CartHeaderDto CartHeader { get; set; }
        public List<CartDetailDto>? CartDetails { get; set; }
    }
}
