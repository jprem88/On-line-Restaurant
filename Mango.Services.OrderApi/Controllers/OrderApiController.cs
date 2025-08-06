using AutoMapper;
using Mango.Services.OrderApi.Data;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Models.Dto;
using Mango.Services.OrderApi.Service.IService;
using Mango.Services.OrderApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mongo.MessageBus;
using Stripe;
using Stripe.Checkout;

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
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public OrderApiController(IMapper mapper, AppDbContext appDbContext, IProductService productService, IMessageBus messageBus, IConfiguration configuration)
        {
            _response = new ResponseDto();
            this._prouctService = productService;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<List<OrderDetailsDto>>(cartDto.CartDetails);
                var order = _appDbContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;

                await _appDbContext.SaveChangesAsync();
                orderHeaderDto.OrderHeaderId = order.OrderHeaderId;
                _response.Result = orderHeaderDto;


            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]

        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51RgRt4Cz4uAQqYZbwPiZUjvOq5XOJRJVKF4rinowwgLDOMXYAkmnMLCo9UAs9h4esSVCC5SlI8d82KievH6fMA4900lusDw2Xw";

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApproveUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    
                };
                var discountObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }
                };
                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.ProductPrice * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }

                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                if(stripeRequestDto.OrderHeader.Discount>0)
                {
                    options.Discounts = discountObj;
                }
                var service = new Stripe.Checkout.SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _appDbContext.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripSessionId = session.Id;
                _appDbContext.SaveChanges();
                _response.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]

        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderId)
        {
            try
            {
                OrderHeader orderHeader = _appDbContext.OrderHeaders.First(x => x.OrderHeaderId == orderId);
                StripeConfiguration.ApiKey = "sk_test_51RgRt4Cz4uAQqYZbwPiZUjvOq5XOJRJVKF4rinowwgLDOMXYAkmnMLCo9UAs9h4esSVCC5SlI8d82KievH6fMA4900lusDw2Xw";
                var service = new Stripe.Checkout.SessionService();
                Session session = service.Get(orderHeader.StripSessionId);
                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);
                if(paymentIntent.Status =="succeeded")
                {
                    //  then payemnt was successfull
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;
                    _appDbContext.SaveChanges();
                    RewardDto rewardDto = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };
                    await _messageBus.PublishMessage(rewardDto, _configuration.GetValue<string>("TopicQueueName:OrderCompletedQueue"));
                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<ResponseDto> GetOrders(string userId ="")
        {
            try
            {
                IEnumerable<OrderHeader> orderlistObj;
                if(User.IsInRole(SD.RoleAdmin))
                {
                    orderlistObj = _appDbContext.OrderHeaders.Include(x => x.OrderDetails).OrderByDescending(x=>x.OrderHeaderId);
                }
                else
                {
                    orderlistObj = _appDbContext.OrderHeaders.Include(x => x.OrderDetails).Where(x=>x.UserId ==userId).OrderByDescending(x => x.OrderHeaderId);
                }
                _response.Result = _mapper.Map<List<OrderHeaderDto>>(orderlistObj);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public async Task<ResponseDto> GetOrder(int id)
        {
            try
            {
                var orderHeader = _appDbContext.OrderHeaders.Include(x => x.OrderDetails).First(x => x.OrderHeaderId == id);
                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderid:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderid,[FromBody] string newstatus)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51RgRt4Cz4uAQqYZbwPiZUjvOq5XOJRJVKF4rinowwgLDOMXYAkmnMLCo9UAs9h4esSVCC5SlI8d82KievH6fMA4900lusDw2Xw";
                var orderHeader = _appDbContext.OrderHeaders.First(x => x.OrderHeaderId == orderid);
                if(orderHeader !=null)
                {
                    if(newstatus ==SD.Status_Cancelled)
                    {
                        ///// refund amount to user
                        var option = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId
                        };
                        var service = new RefundService();
                        var refund = service.Create(option);
                    }
                    orderHeader.Status = newstatus;
                    _appDbContext.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
            


    }
}
