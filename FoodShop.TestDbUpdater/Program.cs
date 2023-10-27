using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

var productCategoriesNames = new string[] {
 "Fresh Produce"
,"far"
,"Meat and Poultry"
,"Seafood"
,"Bakery and Bread"
,"Pantry Staples"
,"Snacks and Sweets"
,"Beverages"
,"Frozen Foods"
,"Specialty and Gourmet Items"
};

var productCategories = productCategoriesNames.Select(n => new ProductCategory
{
    Name = n,
    Description = n
}).ToList();

db.ProductCategories.AddRange(productCategories);


var productNames = new string[] {
"Fruits"
,"Vegetables"
,"Berries"
,"Citrus Fruits"
,"Leafy Greens"
,"Root Vegetables"
,"Herbs"
,"Exotic Fruits"
,"Mushrooms"
,"Melons"
,"Tomatoes"
,"Peppers"
,"Onions and Garlic"
,"Avocados"
,"Apples and Pears"
,"Grapes"
,"Tropical Fruits"
,"Stone Fruits"
,"Cucumbers and Zucchini"
,"Corn and Peas"
};


var categoryFreshProduce = productCategories.FirstOrDefault(c => c.Name == "Fresh Produce");

var products = productNames.Select(n => new Product
{
    Category = categoryFreshProduce,
    Name = n,
    Description = n
});

db.Products.AddRange(products);


db.SaveChanges();