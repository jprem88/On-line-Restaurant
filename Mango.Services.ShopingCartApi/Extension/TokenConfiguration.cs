using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Mango.Services.ShopingCartApi.Extension
{
	public static class TokenConfiguration
	{
		public static WebApplicationBuilder AddTokenConfiguration(this WebApplicationBuilder app)
		{
			app.Services.AddSwaggerGen(option =>
			{
				option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Description = "Enter the Bearer Authorization string as follows:'Bearer Genrated-JWT-TOken'",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"

				});
				option.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
					new OpenApiSecurityScheme
					{
						Reference =new OpenApiReference
						{
							Type =ReferenceType.SecurityScheme,
							Id = JwtBearerDefaults.AuthenticationScheme,

						}
					},new string[]{}
					}
				});
			});
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
