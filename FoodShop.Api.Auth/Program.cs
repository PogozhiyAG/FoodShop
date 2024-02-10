using FoodShop.Api.Auth.Data;
using FoodShop.Api.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FoodShop.BuildingBlocks.Configuration.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddFoodShopJwt();


var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

using (var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>()) {
    db.Database.EnsureCreated();
}
app.Run();
