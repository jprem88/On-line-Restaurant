using static Mango.Web.Utlility.SD;

namespace Mango.Web.Models
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.Get;
        public object Data { get; set; }
        public string Url { get; set; }
        public string AccessToken { get; set; }

    }
}
