using FoodShop.Api.Catalog.Services;
using FoodShop.BuildingBlocks.Configuration.Security;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCompression(o =>
{
    o.EnableForHttps = true;
});
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFoodShopJwt();

builder.Services.AddDbContextFactory<FoodShopDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IProductPriceStrategyProvider, ProductPriceStrategyProvider>();
builder.Services.AddScoped<ICustomerProfile, CustomerProfile>();


var app = builder.Build();

app.UseResponseCompression();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

app.Run();
