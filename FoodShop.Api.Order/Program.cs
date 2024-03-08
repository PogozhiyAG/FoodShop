using Microsoft.EntityFrameworkCore;
using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Services;
using FoodShop.Api.Order.Services.Calculation;
using FoodShop.Api.Order.Services.Calculation.Stage;
using FoodShop.BuildingBlocks.Configuration.Security;
using Stripe;
using FoodShop.Catalog.Grpc;
using Microsoft.Net.Http.Headers;
using MassTransit;
using RabbitMQ.Client;
using FoodShop.Api.Order.Services.MassTransit;
using FoodShop.Api.Order.Middleware;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FoodShop.Api.Order.Configuration;

//Is this a bug? https://stackoverflow.com/questions/65706167/weird-no-ip-address-could-be-resolved-in-rabbitmq-net-client
ConnectionFactory.DefaultAddressFamily = System.Net.Sockets.AddressFamily.InterNetwork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddDbContextFactory<OrderDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddFoodShopJwt();
builder.Services.AddScoped<IAuthenticationContext, AuthenticationContext>();


builder.Services.AddMassTransit(c =>
{
    c.AddConsumers(typeof(Program).Assembly);
    c.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMq:Host"], builder.Configuration["RabbitMq:VirtualHost"], h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]);
            h.Password(builder.Configuration["RabbitMq:Password"]);
        });

        configurator.UseConsumeFilter(typeof(JwtAuthenticationConsumeFilter<>), context);

        configurator.ConfigureEndpoints(context);
    });
});

builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddOrderCalculation();




//gRPC
//TODO Extension method
builder.Services
    .AddGrpcClient<ProductCalculator.ProductCalculatorClient>(o =>
    {
        o.Address = new Uri(builder.Configuration["ApiUrls:FoodShop.Api.Catalog.Grpc"]!);
    })
    .AddCallCredentials((context, metadata, sp) =>
    {
        var authenticationContext = sp.GetRequiredService<IAuthenticationContext>();
        metadata.Add(HeaderNames.Authorization, $"Bearer {authenticationContext.Token}");
        return Task.CompletedTask;
    })
    .ConfigureChannel(o =>
    {
        o.UnsafeUseInsecureChannelCallCredentials = true;
    });


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
app.MapGrpcService<OrderServiceGrpc>();
app.MapGrpcReflectionService();

app.UseMiddleware<AuthenticationContextFromHttpContextMiddleware>();

using (var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<OrderDbContext>())
{
    db.Database.EnsureCreated();
}
app.Run();
