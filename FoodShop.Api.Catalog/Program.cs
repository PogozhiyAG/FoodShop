using FoodShop.Api.Catalog.Commands;
using FoodShop.Api.Catalog.GraphQL;
using FoodShop.Api.Catalog.Services;
using FoodShop.BuildingBlocks.Configuration.Security;
using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FoodShop.Api.Catalog.Behaviors;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Azure.Core;
using Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

//TODO: Test
builder.Services.AddResponseCompression(o =>
{
    o.EnableForHttps = true;
});


builder.Services.AddGraphQLServer()
    .AddFiltering()
    .AddProjections()
    .AddSorting()
    .AddAuthorization()
    .RegisterDbContext<FoodShopDbContext>(DbContextKind.Pooled)
    .AddQueryType<ProductsQuery>();

builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(c => {
    c.RegisterServicesFromAssemblyContaining<Program>();
    c.AddOpenBehavior(typeof(CachingPipelineBehavior<,>));
});

builder.Services.Configure<CachingBehaviorOptions<ProductPriceStrategyLinksRequest, ProductPriceStrategyLinksResponse>>(options =>
{
    options.GetCacheKey = _ => "PRODUCT_PRICE_STRATEGY_LINKS";
    options.GetMemoryCacheEntryOptions = (_, _) => new MemoryCacheEntryOptions
    {
        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(10)
    };
});
builder.Services.Configure<CachingBehaviorOptions<UserTokenTypesRequest, UserTokenTypesResponse>>(options =>
{
    options.GetCacheKey = request => $"USER_TOKEN_TYPES/{request.UserName}";
    options.GetMemoryCacheEntryOptions = (_, _) => new MemoryCacheEntryOptions
    {
        AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(45)
    };
});

builder.Services.AddFoodShopJwt();

builder.Services.AddDbContextFactory<FoodShopDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSingleton<IProductPriceStrategyProvider, ProductPriceStrategyProvider>();
builder.Services.AddScoped<IUserTokenTypesProvider, HttpContextUserTokenTypesProvider>();


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
app.MapGrpcService<ProductCalculatorGrpcService>();
app.MapGrpcReflectionService();
app.MapGraphQL();
app.MapGraphQLSchema();


app.Run();
