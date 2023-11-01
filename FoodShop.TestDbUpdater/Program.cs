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

var category = productCategories.First(c => c.Name == "Fresh Produce");

var products = productNames.Select(n => new Product
{
    Category = category,
    Name = n,
    Description = n,
    Price = Math.Round(Convert.ToDecimal(1.75 + Random.Shared.NextDouble() * 2), 1)
});

db.Products.AddRange(products);
db.SaveChanges();





var raw = @"1. Coca-Cola Classic (Soft Drink)
2. Pepsi (Soft Drink)
3. Sprite (Soft Drink)
4. Mountain Dew (Soft Drink)
5. Fanta Orange (Soft Drink)
6. Dr. Pepper (Soft Drink)
7. 7UP (Soft Drink)
8. Root Beer (Soft Drink)
9. Ginger Ale (Soft Drink)
10. Lemonade (Soft Drink)
11. Iced Tea (Soft Drink)
12. Orange Juice (Juice)
13. Apple Juice (Juice)
14. Cranberry Juice (Juice)
15. Pineapple Juice (Juice)
16. Grapefruit Juice (Juice)
17. Mango Juice (Juice)
18. Pomegranate Juice (Juice)
19. Tomato Juice (Juice)
20. Carrot Juice (Juice)
21. Espresso (Coffee)
22. Cappuccino (Coffee)
23. Latte (Coffee)
24. Americano (Coffee)
25. Mocha (Coffee)
26. Macchiato (Coffee)
27. Flat White (Coffee)
28. Frappuccino (Coffee)
29. Green Tea (Tea)
30. Earl Grey Tea (Tea)
31. Chamomile Tea (Tea)
32. Peppermint Tea (Tea)
33. Jasmine Tea (Tea)
34. Oolong Tea (Tea)
35. Rooibos Tea (Tea)
36. Matcha Tea (Tea)
37. Red Bull (Energy Drink)
38. Monster Energy (Energy Drink)
39. Rockstar Energy (Energy Drink)
40. Amp Energy (Energy Drink)
41. Gatorade (Sports Drink)
42. Powerade (Sports Drink)
43. Vitamin Water (Sports Drink)
44. Coconut Water (Sports Drink)
45. Sparkling Water (Water)
46. Flavored Water (Water)
47. Mineral Water (Water)
48. Alkaline Water (Water)
49. Distilled Water (Water)
50. Coconut Milk (Milk Alternative)
51. Almond Milk (Milk Alternative)
52. Soy Milk (Milk Alternative)
53. Rice Milk (Milk Alternative)
54. Cashew Milk (Milk Alternative)
55. Hazelnut Milk (Milk Alternative)
56. Walnut Milk (Milk Alternative)
57. Pistachio Milk (Milk Alternative)
58. Strawberry Milkshake (Milkshake)
59. Chocolate Milkshake (Milkshake)
60. Vanilla Milkshake (Milkshake)
61. Banana Milkshake (Milkshake)
62. Mango Milkshake (Milkshake)
63. Coffee Milkshake (Milkshake)
64. Oreo Milkshake (Milkshake)
65. Peanut Butter Milkshake (Milkshake)
66. Mojito Cocktail (Alcoholic Beverage)
67. Margarita Cocktail (Alcoholic Beverage)
68. Cosmopolitan Cocktail (Alcoholic Beverage)
69. Martini Cocktail (Alcoholic Beverage)
70. Old Fashioned Cocktail (Alcoholic Beverage)
71. Sangria (Alcoholic Beverage)
72. Pina Colada (Alcoholic Beverage)
73. Moscow Mule (Alcoholic Beverage)
74. Mojito Mocktail (Non-Alcoholic Beverage)
75. Shirley Temple (Non-Alcoholic Beverage)
76. Virgin Pina Colada (Non-Alcoholic Beverage)
77. Mango Lassi (Non-Alcoholic Beverage)
78. Strawberry Daiquiri (Non-Alcoholic Beverage)
79. Blueberry Lemonade (Non-Alcoholic Beverage)
80. Watermelon Cooler (Non-Alcoholic Beverage)
81. Peach Iced Tea (Non-Alcoholic Beverage)
82. Raspberry Lemonade (Non-Alcoholic Beverage)
83. Kiwi Smoothie (Smoothie)
84. Pineapple Coconut Smoothie (Smoothie)
85. Berry Blast Smoothie (Smoothie)
86. Green Detox Smoothie (Smoothie)
87. Mango Banana Smoothie (Smoothie)
88. Avocado Spinach Smoothie (Smoothie)
89. Chocolate Protein Smoothie (Smoothie)
90. Peanut Butter Banana Smoothie (Smoothie)
91. Vanilla Protein Shake (Protein Shake)
92. Chocolate Protein Shake (Protein Shake)
93. Strawberry Protein Shake (Protein Shake)
94. Cookies and Cream Protein Shake (Protein Shake)
95. Coffee Protein Shake (Protein Shake)
96. Banana Nut Protein Shake (Protein Shake)
97. Mixed Berry Protein Shake (Protein Shake)
98. Caramel Protein Shake (Protein Shake)
99. Vanilla Chai Latte (Specialty Beverage)
100. Salted Caramel Mocha (Specialty Beverage)";

var mainCategory = productCategories.First(c => c.Name == "Beverages");
var productList = new List<Product>();
var categoryList = new List<ProductCategory>();

foreach (var str in raw.Split(Environment.NewLine))
{
    var dotIndex = str.IndexOf(".");
    var bracketIndex = str.IndexOf("(");
    var productName = str.Substring(dotIndex + 2, bracketIndex - dotIndex - 3);
    var subCatName = str.Substring(bracketIndex + 1, str.Length - bracketIndex - 2);

    var cat = categoryList.FirstOrDefault(c => c.Name == subCatName);
    if(cat == null)
    {
        cat = new ProductCategory { Name = subCatName, ParentCategory = mainCategory };
        categoryList.Add(cat);
    }

    productList.Add(new Product()
    {
        Category = cat,
        Name = productName,
        Description = productName,
        Price = Math.Round(Convert.ToDecimal(3.75 + Random.Shared.NextDouble() * 2.5), 1)
    });

}

db.AddRange(categoryList);
db.AddRange(productList);
db.SaveChanges();





var tagHealth = new Tag() { Name = "Health" };
var tagChristmas = new Tag() { Name = "Christmas" };
db.Tags.Add(tagHealth);
db.Tags.Add(tagChristmas);
db.SaveChanges();

db.ProductTagRelations.Add(new() { Tag = tagHealth, Product = db.Products.First(p => p.Name == "Vegetables") });
db.ProductTagRelations.Add(new() { Tag = tagHealth, Product = db.Products.First(p => p.Name == "Cucumbers and Zucchini") });
db.ProductTagRelations.Add(new() { Tag = tagChristmas, Product = db.Products.First(p => p.Name == "Citrus Fruits") });
db.ProductTagRelations.Add(new() { Tag = tagChristmas, Product = db.Products.First(p => p.Name == "Fruits") });
db.SaveChanges();

var tokenTypeClub = new TokenType() { Code = "Club", Decription = "Food Club Member" };
db.Add(tokenTypeClub);
db.SaveChanges();

var userToken = new UserToken() { TokenType = tokenTypeClub, UserId = "pogozhiyag@gmail.com", ValidFrom = DateTime.Now.Date, ValidTo = DateTime.Now.AddYears(1).Date };
db.Add(userToken);
db.SaveChanges();

var ps_0_95 = new ProductPriceStrategy() { Quantity = 1, Rate = 0.95M, Name= "5% off" };
var ps_2_90 = new ProductPriceStrategy() { Quantity = 2, Rate = 0.90M, Name= "10% off for 2" };
var ps_0_97 = new ProductPriceStrategy() { Quantity = 1, Rate = 0.97M, Name= "3% off" };
var ps_2_85 = new ProductPriceStrategy() { Quantity = 2, Rate = 0.85M, Name = "15% off" };
db.AddRange(ps_0_95, ps_2_90, ps_0_97, ps_2_85);
db.SaveChanges();


db.Add(new ProductPriceStrategyLink() { ReferenceType = EntityTypeCode.Product, ReferenceId = db.Products.First(p => p.Name == "Avocados").Id, ProductPriceStrategy = ps_0_95 });
db.Add(new ProductPriceStrategyLink() { ReferenceType = EntityTypeCode.Product, ReferenceId = db.Products.First(p => p.Name == "Grapes").Id, ProductPriceStrategy = ps_2_90 });
db.Add(new ProductPriceStrategyLink() { TokenTypeId = tokenTypeClub.Id, ReferenceType = EntityTypeCode.ProductCategory, ReferenceId = productCategories.First(c => c.Name == "Fresh Produce").Id, ProductPriceStrategy = ps_0_97 });
db.Add(new ProductPriceStrategyLink() { TokenTypeId = tokenTypeClub.Id, ReferenceType = EntityTypeCode.Tag, ReferenceId = tagHealth.Id, ProductPriceStrategy = ps_2_85 });
db.SaveChanges();