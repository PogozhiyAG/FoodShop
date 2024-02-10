using Microsoft.EntityFrameworkCore;
using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Services;
using FoodShop.Api.Order.Services.Calculation;
using FoodShop.Api.Order.Services.Calculation.Stage;
using FoodShop.BuildingBlocks.Configuration.Security;
using Stripe;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddDbContextFactory<OrderDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddFoodShopJwt();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddScoped<IProductCatalog, ProductCatalog>();
builder.Services.AddScoped<IOrderCalculator, OrderCalculator>();
builder.Services.AddScoped<ICustomerProfile, CustomerProfile>();

builder.Services.AddSingleton<IOrderAmountCorrectionsProvider, OrderAmountCorrectionsProvider>();

builder.Services.AddKeyedScoped<IOrderCalculationStage, ProductCalculationStage>(ProductCalculationStage.DEFAULT_SERVICE_KEY);
builder.Services.AddKeyedScoped<IOrderCalculationStage, PackingServiceCalculationStage>(PackingServiceCalculationStage.DEFAULT_SERVICE_KEY);
builder.Services.AddKeyedScoped<IOrderCalculationStage, DeliveryCalculationStage>(DeliveryCalculationStage.DEFAULT_SERVICE_KEY);
builder.Services.AddKeyedScoped<IOrderCalculationStage, CorrectionCalculationStage>(CorrectionCalculationStage.DEFAULT_SERVICE_KEY);


//Stripe services
StripeConfiguration.ApiKey = builder.Configuration["Stripe:ApiKey"];
builder.Services.AddTransient<PaymentIntentService>();


var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

using (var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<OrderDbContext>())
{
    db.Database.EnsureCreated();
}
app.Run();
