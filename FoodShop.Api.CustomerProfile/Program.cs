using FoodShop.Api.CustomerProfile.Data;
using Microsoft.EntityFrameworkCore;
using FoodShop.BuildingBlocks.Configuration.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddDbContext<CustomerProfileDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddFoodShopJwt();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<CustomerProfileDbContext>())
{
    db.Database.EnsureCreated();
}
app.Run();
