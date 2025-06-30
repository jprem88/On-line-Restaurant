using Mango.Services.AuthApi.Models;

namespace Mango.Services.AuthApi.Service.IService
{
    public interface IJwtTokenGenratorcs
    {
        string GenrateToken (ApplicationUser user,IEnumerable<string> roles);
    }
}
