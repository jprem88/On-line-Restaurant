using Mango.Services.ProductApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductApi.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(new Product
			{
                ProductId =1,
                Name = "Pizza",
                Description = "vloceno loaded with cheez",
                Price = 1000,
                Category ="",
                ImageUrl =""
            });
            modelBuilder.Entity<Product>().HasData(new Product
			{
				ProductId = 2,
				Name = "Pizza",
				Description = "paneer with capsicum",
				Price = 700,
				Category = "",
				ImageUrl = ""
			});

        }
    }
}
