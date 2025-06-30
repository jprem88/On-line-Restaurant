using AutoMapper;
using Mango.Services.ProductApi.Data;
using Mango.Services.ProductApi.Models;
using Mango.Services.ProductApi.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductApi.Controllers
{
    [Route("api/product")]
    [ApiController]
 
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private ResponseDto responseDto;
        private readonly IMapper _mapper;
        public ProductController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            responseDto = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                var result = _appDbContext.Products.ToList();
                var mappedresult =_mapper.Map<List<ProductDto>>(result);
                responseDto.Result = mappedresult;
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                var result = _appDbContext.Products.FirstOrDefault(x=>x.ProductId ==id);

                responseDto.Result = _mapper.Map<ProductDto>(result);
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }


        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ResponseDto Post([FromBody] ProductDto Product)
        {
            try
            {
                var convertedProduct = _mapper.Map<Product>(Product);
                _appDbContext.Products.Add(convertedProduct);
                _appDbContext.SaveChanges();
                responseDto.Result = _mapper.Map<ProductDto>(convertedProduct);

            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }

        [HttpPut]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Put([FromBody] ProductDto Product)
        {
            try
            {
                var convertedProduct = _mapper.Map<Product>(Product);
                _appDbContext.Products.Update(convertedProduct);
                _appDbContext.SaveChanges();
                responseDto.Result = _mapper.Map<ProductDto>(convertedProduct);

            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }   

        [HttpDelete]
		[Authorize(Roles = "ADMIN")]
		[Route("{id:int}")]

        public ResponseDto Delete(int id)
        {
            try
            {
                var result = _appDbContext.Products.First(x => x.ProductId == id);
                _appDbContext.Products.Remove(result);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }
    }
}
