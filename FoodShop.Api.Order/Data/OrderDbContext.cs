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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Model.Order>().HasIndex(o => o.UserId);
    }
}
