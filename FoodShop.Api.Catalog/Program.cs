using FoodShop.Api.Catalog.GraphQL;
using FoodShop.Api.Catalog.Services;
using FoodShop.BuildingBlocks.Configuration.Security;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

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

builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddFoodShopJwt();

builder.Services.AddDbContextFactory<FoodShopDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IProductPriceStrategyProvider, ProductPriceStrategyProvider>();
builder.Services.AddScoped<ICustomerProfile, CustomerProfile>();
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
