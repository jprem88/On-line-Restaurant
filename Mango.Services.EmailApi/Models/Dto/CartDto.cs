using Mango.Services.EmailApi.Models;

namespace Mango.Services.EmailApi.Models.Dto
{
    public class CartDto
    {
        public CartHeaderDto CartHeader { get; set; }
        public List<CartDetailDto> CartDetails { get; set; }
    }
}
