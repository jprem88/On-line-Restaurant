using Mango.Web.Service.IService;
using Mango.Web.Utlility;

namespace Mango.Web.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            this._contextAccessor = contextAccessor;
        }
        public void ClearToken()
        {
            _contextAccessor.HttpContext.Response.Cookies.Delete(SD.TokenCookie);
        }

        public string? GetToken()
        {
            bool isHasToken = false;
            string ? token = null;
            isHasToken = _contextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.TokenCookie, out token);
            return isHasToken is true ? token : null;
        }

        public void SetToken(string token)
        {
            _contextAccessor.HttpContext.Response.Cookies.Append(SD.TokenCookie, token);
        }
    }
}
