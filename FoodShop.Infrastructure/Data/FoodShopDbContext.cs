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
    public DbSet<BasketItem> BasketItems { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ProductTagRelation> ProductTagRelations { get; set; }
    public DbSet<ProductPriceStrategy> ProductPriceStrategies { get; set; }
    public DbSet<ProductPriceStrategyLink> ProductPriceStrategyLinks { get; set; }
    public DbSet<Brand> Brands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasIndex("Price", "Id");
        modelBuilder.Entity<Product>().HasIndex("Popularity", "Id");
        modelBuilder.Entity<Product>().HasIndex("CustomerRating", "Id");
        modelBuilder.Entity<Product>().HasIndex("CategoryId","Price", "Id");
        modelBuilder.Entity<Product>().HasIndex("CategoryId","Popularity", "Id");
        modelBuilder.Entity<Product>().HasIndex("CategoryId", "CustomerRating", "Id");
        modelBuilder.Entity<Product>().HasIndex("BrandId", "Price", "Id");
        modelBuilder.Entity<Product>().HasIndex("BrandId", "Popularity", "Id");
        modelBuilder.Entity<Product>().HasIndex("BrandId", "CustomerRating", "Id");

        modelBuilder.Entity<Basket>().HasIndex(b => b.OwnerId);

        modelBuilder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();

        modelBuilder.Entity<ProductTagRelation>().HasKey("ProductId", "TagId");

        modelBuilder.Entity<ProductPriceStrategyLink>().HasIndex("TokenTypeCode", "ReferenceType", "ReferenceId");
    }
}
