using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utlility;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Mango.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //////// configure http client
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            //builder.Services.AddHttpClient<ICouponService,CouponService>();
            //builder.Services.AddHttpClient<IAuthService,AuthService>();
            //builder.Services.AddHttpClient<IProductService,ProductService>();
            SD.CouponApiBase = builder.Configuration["ServiceUrls:CouponApi"]!;
            SD.AuthApiBase = builder.Configuration["ServiceUrls:AuthApi"]!;
            SD.ProductApiBase = builder.Configuration["ServiceUrls:ProductApi"]!;
            SD.ShopingCartApiBase = builder.Configuration["ServiceUrls:CartApi"]!;
            SD.OrderApiBase = builder.Configuration["ServiceUrls:OrderApi"]!;


            ///////  configure services
        
            builder.Services.AddScoped<ICouponService, CouponService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBaseService, BaseService>();
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.ExpireTimeSpan = TimeSpan.FromHours(10);
                    option.LoginPath = "/Auth/Login";
                    option.AccessDeniedPath = "/Auth/AccessDenied";
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}