using AutoMapper;
using Mango.Services.ShopingCartApi.Data;
using Mango.Services.ShopingCartApi.Models;
using Mango.Services.ShopingCartApi.Models.Dto;
using Mango.Services.ShopingCartApi.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mongo.MessageBus;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShopingCartApi.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private ResponseDto _responseDto;
        private IMapper _mapper;
        private readonly AppDbContext _appDbContext;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public CartController(IMapper mapper, AppDbContext appDbContext,
            IProductService productService,
            ICouponService couponService,
            IMessageBus messageBus, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _responseDto = new ResponseDto();
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]

        public async Task<ResponseDto> GetCart(string userId)
        {

            try
            {
                var cartHeaderDetails = await _appDbContext.CartHeaders.FirstAsync(x=>x.UserId == userId);
                if (cartHeaderDetails != null) {

                    var cartDetails = new CartDto
                    {
                        CartHeader = _mapper.Map<CartHeaderDto>(cartHeaderDetails),
                        CartDetails = _mapper.Map<List<CartDetailDto>>(_appDbContext.CartDetails.Where(x=>x.CartHeaderId ==cartHeaderDetails.CartHeaderId))
                        
                    };

                    double totalvalue = 0;
                    foreach (var item in cartDetails.CartDetails) {

                        var product = await _productService.GetProducts();
                        item.Product = product.FirstOrDefault(x=>x.ProductId == item.ProductId);
                        totalvalue += (item.Product.Price * item.Count);
                    
                    }

                    cartDetails.CartHeader.CartTotal = totalvalue;
                    if(!string.IsNullOrEmpty(cartDetails.CartHeader.CouponCode))
                    {
                        var couponDetails = await _couponService.GetCouponDetails(cartDetails.CartHeader.CouponCode);
                        if (cartDetails.CartHeader.CartTotal >= couponDetails.MinAmount) {

                            cartDetails.CartHeader.Discount = couponDetails.DiscountAmount;
                            cartDetails.CartHeader.CartTotal = cartDetails.CartHeader.CartTotal - couponDetails.DiscountAmount;
                        
                        }
                    }
                    _responseDto.Result = cartDetails;
                }
                else
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = "No cart available";
                }

                

            }
            catch(Exception ex) {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;

            }
            return _responseDto;

        }

        [HttpPost]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var checkExisting = await _appDbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(x=>x.UserId ==cartDto.CartHeader.UserId);
                if (checkExisting == null) {

                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _appDbContext.Add(cartHeader);
                  await  _appDbContext.SaveChangesAsync();
                    var cartDetails = cartDto.CartDetails.First();
                    cartDetails.CartHeaderId = cartHeader.CartHeaderId;
                    _appDbContext.CartDetails.Add(_mapper.Map<CartDetail>(cartDetails));
                  await  _appDbContext.SaveChangesAsync();
                }
                else
                {
                    var addedProduct = await _appDbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == cartDto.CartDetails.First().ProductId && x.CartHeaderId == checkExisting.CartHeaderId);
                    if (addedProduct == null) {
                        var cartDetails = cartDto.CartDetails.First();
                        cartDetails.CartHeaderId = checkExisting.CartHeaderId;
                        _appDbContext.CartDetails.Add(_mapper.Map<CartDetail>(cartDetails));
                        await _appDbContext.SaveChangesAsync();
                        ////////////////// add new cart details
                    }
                    else
                    {
                        /////////////  update product count
                        var cartDetails = cartDto.CartDetails.First();
                        cartDetails.CartHeaderId = checkExisting.CartHeaderId;
                        cartDetails.Count += addedProduct.Count;
                        cartDetails.CartDetailsId = addedProduct.CartDetailsId;
                        _appDbContext.CartDetails.Update(_mapper.Map<CartDetail>(cartDetails));
                       await _appDbContext.SaveChangesAsync();

                    }
                }
                _responseDto.Result = cartDto;
            }
            catch (Exception ex) {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost("RemoveItem")]

        public async Task<ResponseDto> RemoveItem([FromBody] int cartDetailsId)
        {
            try
            {
                var existingCardDetails = _appDbContext.CartDetails.FirstOrDefault(x => x.CartDetailsId == cartDetailsId);
                if (existingCardDetails != null)
                {

                    _appDbContext.CartDetails.Remove(existingCardDetails);
                    var count = _appDbContext.CartDetails.Where(x => x.CartHeaderId == existingCardDetails.CartHeaderId).Count();
                    if (count == 1)
                    {
                        var cardHeader = _appDbContext.CartHeaders.FirstOrDefault(x => x.CartHeaderId == existingCardDetails.CartHeaderId);
                        _appDbContext.CartHeaders.Remove(cardHeader);
                    }

                }
                await _appDbContext.SaveChangesAsync();
                _responseDto.Result = true;
              
            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpPost("ApplyOrRemoveCoupon")]
        public async Task<ResponseDto> ApplyOrRemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cardHeader = await _appDbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == cartDto.CartHeader.UserId);
                if (cardHeader != null) {
                    cardHeader.CouponCode = cartDto.CartHeader.CouponCode;
                    _appDbContext.CartHeaders.Update(cardHeader);
                    await _appDbContext.SaveChangesAsync();
                }

                _responseDto.Result = true;
            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<ResponseDto> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {

                await _messageBus.PublishMessage(cartDto, _configuration.GetValue<string>("TopicQueueName:EmailShopingCart"));
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }


    }
}
