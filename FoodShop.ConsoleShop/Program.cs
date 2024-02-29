
using FoodShop.ConsoleShop;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices(services =>
{
    services.AddMassTransit(c =>
    {
        c.UsingRabbitMq((context, configurator) =>
        {
            configurator.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
            configurator.ConfigureEndpoints(context);
        });
    });

    services.AddHttpClient();

    services.AddHostedService<Application>();
});


var host = builder.Build();
host.Run();

