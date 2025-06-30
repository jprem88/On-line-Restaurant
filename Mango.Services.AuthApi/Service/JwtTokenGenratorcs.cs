using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Models.Dto;
using Mango.Services.AuthApi.Service.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.Services.AuthApi.Service
{
    public class JwtTokenGenratorcs : IJwtTokenGenratorcs
    {
        private readonly JwtOptions _jwtOptions;
       
        public JwtTokenGenratorcs(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;


        }
        public string GenrateToken(ApplicationUser user,IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Sub,user.Id),
                new Claim(JwtRegisteredClaimNames.Name,user.Name)
            };

            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x))); 
            var tokenDescripter = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials =new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescripter);
            return tokenHandler.WriteToken(token);  
        }
    }
}
