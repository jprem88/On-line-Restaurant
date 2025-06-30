using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using static Mango.Web.Utlility.SD;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {

            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;


		}
        public async Task<ResponseDto> SendAsync(RequestDto requestDto, bool isBearer = true)
        {

            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("MangoApi");
                HttpRequestMessage httpRequestMessage = new();
                httpRequestMessage.Headers.Add("Accept", "application/json");
                //////To Do add token 
                if(isBearer)
                {
                    var token = _tokenProvider.GetToken();
                    httpRequestMessage.Headers.Add("Authorization", $"Bearer {token}");
                }
                httpRequestMessage.RequestUri = new Uri(requestDto.Url);
                if (requestDto.Data != null)
                {
                    httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }
                HttpResponseMessage? httpResponse = null;
                switch (requestDto.ApiType)
                {
                    case ApiType.Post:
                        httpRequestMessage.Method = HttpMethod.Post;
                        break;
                    case ApiType.Put:
                        httpRequestMessage.Method = HttpMethod.Put;
                        break;
                    case ApiType.Delete:
                        httpRequestMessage.Method = HttpMethod.Delete;
                        break;
                    default:
                        httpRequestMessage.Method = HttpMethod.Get;
                        break;
                }

                httpResponse = await httpClient.SendAsync(httpRequestMessage);
                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };

                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorised" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Not Authorized for this request" };
                    case HttpStatusCode.BadRequest:
                        return new() { IsSuccess = false, Message = "Bad Request" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server" };
                    default:
                        var content = await httpResponse.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<ResponseDto>(content);
                        return apiResponse;
                }
            }
            catch (Exception ex)
            {
                var respone = new ResponseDto()
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString()

                };
                return respone;
            }

        }
    }
}
