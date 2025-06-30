using Mango.Services.CouponApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponApi.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId =1,
                CouponCode = "100OFF",
                DiscountAmount = 100,
                MinAmount = 1000
            });
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId =2,
                CouponCode = "200OFF",
                DiscountAmount = 200,
                MinAmount = 2000
            });

        }
    }
}
