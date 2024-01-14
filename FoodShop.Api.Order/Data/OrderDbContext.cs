using Microsoft.EntityFrameworkCore;
using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Model.Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderCalculation> OrderCalculations { get; set; }
    public DbSet<OrderCalculationProperty> OrderCalculationProperties { get; set; }
    public DbSet<DeliveryInfo> DeliveryInfos { get; set; }
    public DbSet<OrderAmountCorrection> OrderAmountCorrections { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Model.Order>().HasIndex(o => o.UserId);

        modelBuilder.Entity<OrderCalculationProperty>().HasKey("OrderCalculationId", "Name");
    }
}
