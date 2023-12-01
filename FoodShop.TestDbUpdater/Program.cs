using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;

IServiceCollection services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

services.AddSingleton<IConfiguration>(configuration);
services.AddDbContext<FoodShopDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")
));



var serviceProvider = services.BuildServiceProvider();

using var db = serviceProvider.GetRequiredService<FoodShopDbContext>();
db.Database.EnsureCreated();



db.Products.ExecuteDelete();
db.ProductCategories.ExecuteDelete();


var dicCategories = new Dictionary<string, ProductCategory>();
var dicBrands = new Dictionary<string, Brand>();
var random = new Random(DateTime.Now.Millisecond);
int cnt = 0;

using (FileStream? fileStream = new FileStream(@"C:\OldSchool\Test\ASP\brandedDownload.json ", FileMode.Open))
{

    fileStream.Position = 17;

    var options = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    IAsyncEnumerable<RawProduct?> rawProducts = JsonSerializer.DeserializeAsyncEnumerable<RawProduct?>(fileStream, options);
    try
    {
        await foreach (RawProduct? p in rawProducts)
        {
            if (!dicCategories.TryGetValue(p.BrandedFoodCategory, out var cat))
            {
                dicCategories[p.BrandedFoodCategory] = cat = new ProductCategory()
                {
                    Name = p.BrandedFoodCategory,
                    Description = p.BrandedFoodCategory
                };
                db.ProductCategories.Add(cat);
                //db.SaveChanges();
            }
            if (!dicBrands.TryGetValue(p.BrandOwner, out var brand))
            {
                dicBrands[p.BrandOwner] = brand = new Brand()
                {
                    Name = p.BrandOwner
                };
                db.Brands.Add(brand);
                //db.SaveChanges();
            }
            var newProduct = new Product()
            {
                Name = p.Description,
                Description = p.Ingredients,
                //CategoryId = cat.Id,
                //BrandId = brand.Id,
                Category = cat,
                Brand = brand,
                Price = 0.5M + Convert.ToDecimal(Math.Round(random.NextDouble() * 10, 2)),
                Popularity = 100 * Convert.ToDecimal(random.NextDouble()),
                CustomerRating = 5 * Convert.ToDecimal(random.NextDouble()),
            };
            db.Products.Add(newProduct);

            if (++cnt % 10000 == 0)
            {
                db.SaveChanges();
                Console.WriteLine(cnt);
            }
        }
    }
    catch { };
}

db.SaveChanges();
Console.WriteLine("STOP");






var tagHealth = new Tag() { Name = "Health"};
var tagChristmas = new Tag() { Name = "Christmas" };
db.Tags.Add(tagHealth);
db.Tags.Add(tagChristmas);
db.SaveChanges();

db.ProductTagRelations.Add(new() { Tag = tagHealth, Product = db.Products.First(p => p.Name == "DRIED VEGETABLES") });
db.ProductTagRelations.Add(new() { Tag = tagHealth, Product = db.Products.First(p => p.Name == "MINI CUCUMBERS") });
db.ProductTagRelations.Add(new() { Tag = tagChristmas, Product = db.Products.First(p => p.Name == "ORANGE CRANBERRY SCONES") });
db.ProductTagRelations.Add(new() { Tag = tagChristmas, Product = db.Products.First(p => p.Name == "CONCORD GRAPE JELLY") });
db.SaveChanges();

var tokenTypeClub = "Club";


var ps_0_95 = new ProductPriceStrategy() { Quantity = 1, Rate = 0.95M, Name= "5% off" };
var ps_2_90 = new ProductPriceStrategy() { Quantity = 2, Rate = 0.90M, Name= "10% off for 2" };
var ps_0_97 = new ProductPriceStrategy() { Quantity = 1, Rate = 0.97M, Name= "3% off" };
var ps_2_85 = new ProductPriceStrategy() { Quantity = 2, Rate = 0.85M, Name = "15% off for 2" };
var ps_0_94 = new ProductPriceStrategy() { Quantity = 1, Rate = 0.99M, Name = "4%" };
db.AddRange(ps_0_95, ps_2_90, ps_0_97, ps_2_85, ps_0_94);
db.SaveChanges();


db.Add(new ProductPriceStrategyLink() { Priority = 100, ReferenceType = EntityTypeCode.Product, ReferenceId = db.Products.First(p => p.Name == "MINI AVOCADOS").Id, ProductPriceStrategy = ps_0_95 });
db.Add(new ProductPriceStrategyLink() { Priority = 101, ReferenceType = EntityTypeCode.Product, ReferenceId = db.Products.First(p => p.Name == "GREEN GRAPES").Id, ProductPriceStrategy = ps_2_90 });
db.Add(new ProductPriceStrategyLink() { Priority = 150.1M, TokenTypeCode = tokenTypeClub, ReferenceType = EntityTypeCode.ProductCategory, ReferenceId = db.ProductCategories.First(c => c.Name == "Candy").Id, ProductPriceStrategy = ps_0_97 });
db.Add(new ProductPriceStrategyLink() { Priority = 150.2M, TokenTypeCode = tokenTypeClub, ReferenceType = EntityTypeCode.Tag, ReferenceId = tagHealth.Id, ProductPriceStrategy = ps_2_85 });
db.Add(new ProductPriceStrategyLink() { Priority = 120, ReferenceType = EntityTypeCode.Tag, ReferenceId = tagChristmas.Id, ProductPriceStrategy = ps_0_94 });
db.SaveChanges();





record RawProduct
{
    public string BrandedFoodCategory { get; set; }
    public string Description { get; set; }
    public string BrandOwner { get; set; }
    public string Ingredients { get; set; }
    public decimal ServingSize { get; set; }
    public string ServingSizeUnit { get; set; }
    public string PublicationDate { get; set; }
}