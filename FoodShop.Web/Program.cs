using FoodShop.Infrastructure.Data;
using FoodShop.Web.Data;
using FoodShop.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationIdentityDbContext>();

builder.Services.AddDbContext<FoodShopDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<IUserTokenProvider, UserTokenProvider>();
//builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IBasketService, RedisBasketService>();
builder.Services.AddScoped<IOrderCalculator, OrderCalculator>();
//builder.Services.AddScoped<IProductPriceCalculator, ProductPriceCalculator>();
builder.Services.AddScoped<IProductPriceStrategyProvider, ProductPriceStrategyProvider>();


var app = builder.Build();





if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var identityContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
    identityContext.Database.EnsureCreated();
    var domainDbContext = scope.ServiceProvider.GetRequiredService<FoodShopDbContext>();
    domainDbContext.Database.EnsureCreated();
}


app.Run();





