using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using FoodShop.Api.Order.Configuration;
using Microsoft.EntityFrameworkCore;
using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<OrderDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IConfigureOptions<JwtBearerOptions>, ConfigureJWTBearerOptions>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddScoped<IProductCatalog, ProductCatalog>();
builder.Services.AddScoped<IOrderCalculator, OrderCalculator>();

builder.Services.AddKeyedScoped<IOrderCalculationStage, ProductCalculationStage>(ProductCalculationStage.DEFAULT_SERVICE_KEY);




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<OrderDbContext>())
{
    db.Database.EnsureCreated();
}
app.Run();
