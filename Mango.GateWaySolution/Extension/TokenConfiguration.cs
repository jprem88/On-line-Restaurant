using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace Mango.GateWaySolution.Extension
{
	public static class TokenConfiguration
	{
		public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder app)
		{
			var setting = app.Configuration.GetSection("ApiSettings");
			var secret = setting.GetValue<string>("Secret");
			var issuer = setting.GetValue<string>("Issuer");
			var audiance = setting.GetValue<string>("Audience");
			var key = Encoding.ASCII.GetBytes(secret);
			app.Services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(x =>
			{
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = true,
					ValidIssuer = issuer,
					ValidateAudience = true,
					ValidAudience = audiance,

				};
			});
			return app;
		}
	}
}
