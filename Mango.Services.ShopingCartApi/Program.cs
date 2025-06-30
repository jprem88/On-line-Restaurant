
using AutoMapper;
using Mango.Services.ShopingCartApi.Data;
using Mango.Services.ShopingCartApi.Extension;
using Mango.Services.ShopingCartApi.Service;
using Mango.Services.ShopingCartApi.Service.IService;
using Mango.Services.ShopingCartApi.Utility;
using Microsoft.EntityFrameworkCore;
using Mongo.MessageBus;

namespace Mango.Services.ShopingCartApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("cartconnection"));
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient("Product", x => x.BaseAddress = new Uri(builder.Configuration["ServiceBaseUrl:ProductUrl"])).AddHttpMessageHandler<BackEndApiAuthentication>();
            builder.Services.AddHttpClient("Coupon", x => x.BaseAddress = new Uri(builder.Configuration["ServiceBaseUrl:CouponUrl"])).AddHttpMessageHandler<BackEndApiAuthentication>();
            IMapper mapper = MappingConfig.RegisterMap().CreateMapper();
            builder.Services.AddSingleton(mapper);
            builder.Services.AddScoped<BackEndApiAuthentication>();
            builder.Services.AddScoped<IProductService,ProductService>();
            builder.Services.AddScoped<ICouponService,CouponService>();
            builder.Services.AddScoped<IMessageBus, MessageBus>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.AddTokenConfiguration();

            builder.Services.AddAuthentication();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            ApplyMigration();

            app.Run();

            void ApplyMigration()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    if (_db.Database.GetPendingMigrations().Count() > 0)
                    {
                        _db.Database.Migrate();
                    }
                }
            }
        }
    }
}