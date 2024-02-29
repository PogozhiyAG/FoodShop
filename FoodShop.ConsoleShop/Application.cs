using FoodShop.MessageContracts.Order;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.ConsoleShop;

public class Application : BackgroundService
{
    private Menu mainMenu;
    private Menu currentMenu;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IBus _bus;
    private readonly ILogger<Application> _logger;
    private string token;

    public Application(IHttpClientFactory httpClientFactory, IBus bus, ILogger<Application> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Init();
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Yield();
            await currentMenu.Print();
        }
    }



    private void Init()
    {
        mainMenu = new Menu();
        mainMenu.Commands.AddRange(new Command[] {
            new Command(){ Key = ConsoleKey.D1, Name = "Login", Execute = LoginExecute},
            new Command(){ Key = ConsoleKey.D2, Name = "Order", Execute = OrderExecute}
        });

        currentMenu = mainMenu;
    }

    private async Task LoginExecute()
    {
        Console.WriteLine("LoginExecute");
        var httpClient = _httpClientFactory.CreateClient();
        var url = "https://localhost:11443/Authentication/login";


        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(new { userName = "string", password = "String@1" })
        };

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        _logger.LogInformation(httpResponseMessage.ToString());

        var result = await httpResponseMessage.Content.ReadFromJsonAsync<TokenResponse>();
        token = result.Token;
        _logger.LogInformation(token);

        await Task.Delay(2000);

    }

    private async Task OrderExecute()
    {
        Console.WriteLine("OrderExecute");

        var createOrder = new CreateOrder(
            Authorization: token,
            Items: new List<OrderItem>() {
                new ("5", 3),
                new ("1700", 6)
            },
            Delivery: new DeliveryInfo("Teddington", DateTime.Now, DateTime.Now, "", "Me"),
            Description: "Console shop"
        );

        var response = await _bus.Request<CreateOrder, CreateOrderResponse>(message: createOrder, callback: ctx =>
        {
            ctx.Headers.Set("token", token);
        });

        Console.WriteLine(response.Message.Id);

        await Task.Delay(1000);

    }
}


public class Command
{
    public ConsoleKey Key { get; set; }
    public string Name { get; set; }
    public override string ToString()
    {
        return $"{Key}. {Name}";
    }

    public Func<Task> Execute { get; set; }
}


public class Menu
{
    public string Name { get; set; }
    public List<Command> Commands { get; set; } = [];

    public async Task Print()
    {
        Console.Clear();
        foreach (var command in Commands)
        {
            Console.WriteLine(command);
        }

        var key = Console.ReadKey(true);
        var currentCommand = Commands.FirstOrDefault(c => c.Key == key.Key);

        if(currentCommand != null)
        {
            await currentCommand.Execute();
        }

    }
}



public class TokenResponse
{
    public required string Token { get; set; }
    public string? RefreshToken { get; set; }
    public bool IsAnonymous { get; set; }
    public string? UserName { get; set; }
}