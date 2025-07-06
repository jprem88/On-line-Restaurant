using AutoMapper;
using Mango.Services.OrderApi.Data;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Models.Dto;
using Mango.Services.OrderApi.Service.IService;
using Mango.Services.OrderApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.OrderApi.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {
        protected ResponseDto _response;
        private IMapper _mapper;
        private IProductService _prouctService;
        private readonly AppDbContext _appDbContext;
        public OrderApiController(IMapper mapper, AppDbContext appDbContext,IProductService productService)
        {
            _response = new ResponseDto();
            this._prouctService = productService;
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async  Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<List<OrderDetailsDto>>(cartDto.CartDetails);
                var order = _appDbContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;

                orderHeaderDto.OrderHeaderId = order.OrderHeaderId;
                _response.Result = orderHeaderDto;

             await  _appDbContext.SaveChangesAsync();
                

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
