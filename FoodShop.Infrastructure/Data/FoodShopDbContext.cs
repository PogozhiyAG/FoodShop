using FoodShop.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Infrastructure.Data;

public class FoodShopDbContext : DbContext
{
    public FoodShopDbContext(DbContextOptions<FoodShopDbContext> options) : base(options)
    {
    }

    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
}
