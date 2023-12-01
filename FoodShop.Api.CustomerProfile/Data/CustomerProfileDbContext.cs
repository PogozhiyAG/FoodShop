using FoodShop.Api.CustomerProfile.Model;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.CustomerProfile.Data;

public class CustomerProfileDbContext : DbContext
{
    public CustomerProfileDbContext(DbContextOptions<CustomerProfileDbContext> options) : base(options)
    {
    }

    public DbSet<CustomerTokenType> CustomerTokenTypes { get; set; }
    public DbSet<CustomerToken> CustomerTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerTokenType>().HasIndex(t => t.Code).IsUnique();
    }
}